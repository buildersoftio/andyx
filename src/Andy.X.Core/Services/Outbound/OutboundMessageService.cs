﻿using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Subscriptions;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.Core.Contexts.Subscriptions;
using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Services.Outbound
{
    public class OutboundMessageService : IOutboundMessageService
    {
        private readonly ILogger<OutboundMessageService> _logger;
        private readonly ISubscriptionHubRepository _subscriptionHubRepository;
        private readonly ISubscriptionHubService _subscriptionHubService;
        private readonly StorageConfiguration _storageConfiguration;

        private readonly ConcurrentDictionary<string, SubscriptionTopicData> _subscriptionTopicData;

        public OutboundMessageService(
            ILogger<OutboundMessageService> logger,
            ISubscriptionHubRepository subscriptionHubRepository,
            ISubscriptionHubService subscriptionHubService,
            StorageConfiguration storageConfiguration)
        {
            _logger = logger;
            _subscriptionHubRepository = subscriptionHubRepository;
            _subscriptionHubService = subscriptionHubService;
            _storageConfiguration = storageConfiguration;
            _subscriptionTopicData = new ConcurrentDictionary<string, SubscriptionTopicData>();
        }

        public async Task AddSubscriptionTopicData(SubscriptionTopicData subscriptionTopicData)
        {
            var subscriptionId = ConnectorHelper.GetSubcriptionId(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName);
            if (_subscriptionTopicData.ContainsKey(subscriptionId) != true)
            {
                using (var subDbContext = new SubscriptionPositionContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName))
                {
                    subscriptionTopicData.CurrentPosition = subDbContext.CurrentPosition.OrderBy(x => x.SubscriptionName).FirstOrDefault();

                    subscriptionTopicData.LastLedgerPositionInQueue = subscriptionTopicData.CurrentPosition.ReadLedgerPosition;
                    subscriptionTopicData.LastEntryPositionInQueue = subscriptionTopicData.CurrentPosition.ReadEntryPosition;
                    subscriptionTopicData.LastPositionUnackedInQueue = 0;
                    subscriptionTopicData.LastEntryUnackedInLog = GetLastEntryFromUnckedMessageLogs(subscriptionTopicData);
                }

                _subscriptionTopicData.TryAdd(subscriptionId, subscriptionTopicData);

                subscriptionTopicData.DoesUnackedMessagesExists = CheckUnackedMessagesIfExists(subscriptionId);
                subscriptionTopicData.StoringCurrentPosition += SubscriptionTopicData_TriggerLogic;
                subscriptionTopicData.ReadMessagesFromStorage += SubscriptionTopicData_ReadMessagesFromStorage;
            }
            subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            subscriptionTopicData.StartService();

            // if sub type is Unique.
            // load 100 rows into memory
            // In case of shared subscription, if there exists a consumer conencted
            if (subscriptionTopicData.IsConsuming != true)
            {
                if (subscriptionTopicData.Subscription.SubscriptionMode == SubscriptionMode.Resilient)
                {
                    LoadNext100MessagesInMemory(subscriptionId);
                    await SendFirstMessage(subscriptionId, subscriptionTopicData.CurrentPosition.ReadLedgerPosition, subscriptionTopicData.CurrentPosition.ReadEntryPosition);
                }
                else
                {
                    if (LoadNext100UnacknowledgedMessagesInMemory(subscriptionId) == true)
                    {
                        await SendAllMessages(subscriptionId, true);
                    }
                    else
                    {
                        LoadNext100MessagesInMemory(subscriptionId);
                        await SendAllMessages(subscriptionId);
                    }
                }
            }
        }

        public async Task SendFirstMessage(string subscriptionId, long currentLedgerId, long currentEntryId)
        {
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];

            var isFirstMessageDequeued = subscriptionTopic.TemporaryMessageQueue.TryDequeue(out string firstMessage, out DateTimeOffset priority);
            if (isFirstMessageDequeued == true)
            {
                subscriptionTopic.SetConsumingFlag();

                var message = subscriptionTopic.TemporaryMessages[firstMessage];

                await UpdateCurrentPosition(subscriptionId, currentLedgerId, currentEntryId);


                await _subscriptionHubService.TransmitMessage(subscriptionTopic.Subscription.Tenant,
                    subscriptionTopic.Subscription.Product,
                    subscriptionTopic.Subscription.Component,
                    subscriptionTopic.Subscription.Topic,
                    subscriptionTopic.Subscription.SubscriptionName,
                    message);
            }
        }

        public async Task SendNextMessage(string subscriptionId, long currentLedgerId, long currentEntryId)
        {
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];

            var isNextMessageDequeued = subscriptionTopic.TemporaryMessageQueue.TryDequeue(out string nextMessage, out DateTimeOffset priority);
            if (isNextMessageDequeued == true)
            {
                var message = subscriptionTopic.TemporaryMessages[nextMessage];

                await UpdateCurrentPosition(subscriptionId, message.LedgerId, message.Id);

                // delete message from memory
                subscriptionTopic.TemporaryMessages.TryRemove($"{currentLedgerId}:{currentEntryId}", out _);

                await _subscriptionHubService.TransmitMessage(subscriptionTopic.Subscription.Tenant,
                    subscriptionTopic.Subscription.Product,
                    subscriptionTopic.Subscription.Component,
                    subscriptionTopic.Subscription.Topic,
                    subscriptionTopic.Subscription.SubscriptionName,
                    subscriptionTopic.TemporaryMessages[nextMessage]);

            }
            else
            {
                if (currentEntryId == _storageConfiguration.LedgerSize)
                {
                    subscriptionTopic.CurrentPosition.ReadLedgerPosition = subscriptionTopic.CurrentPosition.ReadLedgerPosition + 1;
                    subscriptionTopic.CurrentPosition.ReadEntryPosition = 0;
                }

                subscriptionTopic.UnsetConsumingFlag();
            }

            // check if messages are 50% in the queue.
            if (subscriptionTopic.TemporaryMessageQueue.Count == 50)
                LoadNext100MessagesInMemory(subscriptionId);
        }

        public async Task SendSameMessage(string subscriptionId, long currentLedgerId, long currentEntryId)
        {
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];

            await _subscriptionHubService.TransmitMessage(subscriptionTopic.Subscription.Tenant,
                subscriptionTopic.Subscription.Product,
                subscriptionTopic.Subscription.Component,
                subscriptionTopic.Subscription.Topic,
                subscriptionTopic.Subscription.SubscriptionName,
                subscriptionTopic.TemporaryMessages[$"{currentLedgerId}:{currentEntryId}"]);
        }

        public Task StartOutboundMessageServiceForSubscription(string subscriptionId)
        {
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];
            subscriptionTopic.StartService();

            return Task.CompletedTask;
        }

        public Task StopOutboundMessageServiceForSubscription(string subscriptionId)
        {
            var sub = _subscriptionHubRepository.GetSubscriptionById(subscriptionId);
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];

            if (sub.ConsumersConnected.Count == 0)
            {

                subscriptionTopic.StopService();
                subscriptionTopic.IsOutboundServiceRunning = false;
                subscriptionTopic.IsConsuming = false;

                // remove all messages from memory, current position is saved
                subscriptionTopic.LastPositionUnackedInQueue = 0;
                subscriptionTopic.TemporaryMessageQueue.Clear();
                subscriptionTopic.TemporaryMessages.Clear();

                ReleaseMemory(subscriptionId);
            }

            return Task.CompletedTask;
        }


        public Task StoreCurrentPositionAsync(string subscriptionId)
        {
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            using (var subDbContext = new SubscriptionPositionContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName))
            {
                var subPositon = subDbContext.CurrentPosition.OrderBy(x => x.SubscriptionName).FirstOrDefault();

                if (subPositon.ReadLedgerPosition != subscriptionTopicData.CurrentPosition.ReadLedgerPosition || subPositon.ReadEntryPosition != subscriptionTopicData.CurrentPosition.ReadEntryPosition)
                {
                    subPositon.ReadLedgerPosition = subscriptionTopicData.CurrentPosition.ReadLedgerPosition;
                    subPositon.ReadEntryPosition = subscriptionTopicData.CurrentPosition.ReadEntryPosition;
                    subPositon.UpdatedDate = DateTimeOffset.Now;

                    subDbContext.CurrentPosition.Update(subPositon);
                    subDbContext.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }

        public Task TriggerSubscriptionsByProducer(string tenant, string product, string component, string topic)
        {
            var subscriptions = _subscriptionHubRepository.GetSubscriptionsByTopic(tenant, product, component, topic);
            foreach (var subscription in subscriptions)
            {
                if (subscription.Value.ConsumersConnected.Count == 0)
                    continue;

                var subTopic = _subscriptionTopicData[subscription.Key];
                if (subTopic.IsConsuming != true)
                {
                    if (LoadNext100MessagesInMemory(subscription.Key) == true)
                    {
                        if (subTopic.Subscription.SubscriptionMode == SubscriptionMode.Resilient)
                        {
                            Task.Run(async () => await SendFirstMessage(subscription.Key, subTopic.CurrentPosition.ReadLedgerPosition, subTopic.CurrentPosition.ReadEntryPosition));
                        }
                        else
                        {
                            Task.Run(async () => await SendAllMessages(subscription.Key));
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task UpdateCurrentPosition(string subscriptionId, long currentLedgerId, long currentEntryId)
        {
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];
            subscriptionTopicData.CurrentPosition.ReadLedgerPosition = currentLedgerId;
            subscriptionTopicData.CurrentPosition.ReadEntryPosition = currentEntryId;

            return Task.CompletedTask;
        }

        public async Task SendAllMessages(string subscriptionId, bool sendUnackedMessage = false)
        {
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];
            subscriptionTopic.SetConsumingFlag();
            while (subscriptionTopic.TemporaryMessageQueue.TryDequeue(out string nextMessage, out DateTimeOffset priority))
            {
                var message = subscriptionTopic.TemporaryMessages[nextMessage];

                // TODO: Test with 'shared' subscription type.
                // we are updating the current position when the consumer is responsing that if it got the message.
                //if (sendUnackedMessage != true)
                //    await UpdateCurrentPosition(subscriptionId, message.LedgerId, message.Id);

                await _subscriptionHubService.TransmitMessage(subscriptionTopic.Subscription.Tenant,
                    subscriptionTopic.Subscription.Product,
                    subscriptionTopic.Subscription.Component,
                    subscriptionTopic.Subscription.Topic,
                    subscriptionTopic.Subscription.SubscriptionName,
                    message);

                // delete message from memory
                subscriptionTopic.TemporaryMessages.TryRemove($"{message.LedgerId}:{message.Id}", out _);

                // reading new messages from disk
                // check if messages are 50% in the queue.
                if (subscriptionTopic.TemporaryMessageQueue.Count == 50)
                {
                    if (sendUnackedMessage == true)
                        LoadNext100UnacknowledgedMessagesInMemory(subscriptionId);
                    else
                        LoadNext100MessagesInMemory(subscriptionId);
                }

                if (subscriptionTopic.IsConsuming == false)
                    break;
            }

            if (subscriptionTopic.TemporaryMessageQueue.Count == 0 && sendUnackedMessage == true)
                subscriptionTopic.DoesUnackedMessagesExists = false;

            subscriptionTopic.UnsetConsumingFlag();
        }

        public Task LoadSubscriptionTopicDataInMemory(SubscriptionTopicData subscriptionTopicData)
        {
            var subscriptionId = ConnectorHelper.GetSubcriptionId(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName);
            if (_subscriptionTopicData.ContainsKey(subscriptionId) != true)
            {
                using (var subDbContext = new SubscriptionPositionContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName))
                {
                    subscriptionTopicData.CurrentPosition = subDbContext.CurrentPosition.OrderBy(x => x.SubscriptionName).FirstOrDefault();

                    subscriptionTopicData.LastLedgerPositionInQueue = subscriptionTopicData.CurrentPosition.ReadLedgerPosition;
                    subscriptionTopicData.LastEntryPositionInQueue = subscriptionTopicData.CurrentPosition.ReadEntryPosition;
                    subscriptionTopicData.LastPositionUnackedInQueue = 0;
                    subscriptionTopicData.LastEntryUnackedInLog = GetLastEntryFromUnckedMessageLogs(subscriptionTopicData);
                }


                _subscriptionTopicData.TryAdd(subscriptionId, subscriptionTopicData);

                subscriptionTopicData.DoesUnackedMessagesExists = CheckUnackedMessagesIfExists(subscriptionId);
                subscriptionTopicData.StoringCurrentPosition += SubscriptionTopicData_TriggerLogic;
                subscriptionTopicData.ReadMessagesFromStorage += SubscriptionTopicData_ReadMessagesFromStorage;
            }

            return Task.CompletedTask;
        }

        private long GetLastEntryFromUnckedMessageLogs(SubscriptionTopicData subscriptionTopicData)
        {
            using (var ackedDbContext = new MessageAcknowledgementContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName))
            {
                var lastEntry = ackedDbContext.UnacknowledgedMessages.OrderBy(x => x.Id).LastOrDefault();
                if (lastEntry != null)
                    return lastEntry.Id;
            }
            return 0;
        }

        private bool LoadNext100MessagesInMemory(string subscriptionId)
        {
            bool isMemoryLoaded = true;
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            using (var storageContext = new StorageContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.LastLedgerPositionInQueue))
            {
                var newMessages = storageContext.Messages.Where(x => x.Id > subscriptionTopicData.LastEntryPositionInQueue).OrderBy(o => o.Id).Take(100);

                foreach (var msg in newMessages)
                {
                    subscriptionTopicData.TemporaryMessageQueue.TryEnqueue($"{msg.LedgerId}:{msg.Id}", msg.SentDate);
                    subscriptionTopicData.TemporaryMessages.TryAdd($"{msg.LedgerId}:{msg.Id}", msg);

                    subscriptionTopicData.LastLedgerPositionInQueue = msg.LedgerId;
                    subscriptionTopicData.LastEntryPositionInQueue = msg.Id;
                }

                if (newMessages.Count() >= 0)
                {
                    if (subscriptionTopicData.LastEntryPositionInQueue == _storageConfiguration.LedgerSize)
                    {
                        // prepare ledger change for the next file
                        subscriptionTopicData.LastLedgerPositionInQueue = subscriptionTopicData.LastLedgerPositionInQueue + 1;
                        subscriptionTopicData.LastEntryPositionInQueue = 0;
                    }
                }

                if (newMessages.Count() == 0)
                    isMemoryLoaded = false;
            }

            return isMemoryLoaded;
        }

        private bool LoadNext100UnacknowledgedMessagesInMemory(string subscriptionId)
        {
            bool isMemoryLoaded = true;
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];
            subscriptionTopicData.DoesUnackedMessagesExists = true;

            using (var unackedContext = new MessageAcknowledgementContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName))
            {
                int take = 100;
                var currentLedgerId = subscriptionTopicData.LastLedgerPositionInQueue;
                if ((subscriptionTopicData.LastEntryUnackedInLog - subscriptionTopicData.LastPositionUnackedInQueue) <= 100)
                {
                    take = (int)subscriptionTopicData.LastEntryUnackedInLog - (int)subscriptionTopicData.LastPositionUnackedInQueue;
                }
                var newUnackMessages = unackedContext.UnacknowledgedMessages.Where(x => x.Id > subscriptionTopicData.LastPositionUnackedInQueue).OrderBy(x => x.Id).Take(take).ToList();
                var ledgers = newUnackMessages.Select(x => x.LedgerId).ToList().Distinct();
                foreach (var ledger in ledgers)
                {
                    var listOfEntries = newUnackMessages.Where(x => x.LedgerId == ledger).Select(x => x.EntryId).ToList();
                    using (var storageContext = new StorageContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, ledger))
                    {
                        var messages = storageContext.Messages.Where(x => x.LedgerId == ledger && listOfEntries.Contains(x.Id));
                        foreach (var msg in messages)
                        {
                            subscriptionTopicData.TemporaryMessageQueue.TryEnqueue($"{msg.LedgerId}:{msg.Id}", msg.SentDate);
                            subscriptionTopicData.TemporaryMessages.TryAdd($"{msg.LedgerId}:{msg.Id}", msg);

                            subscriptionTopicData.TemporaryUnackedMessageIds.TryAdd($"{msg.LedgerId}:{msg.Id}", msg.Id);
                        }
                    }
                }

                if (newUnackMessages.Count() != 0)
                {
                    subscriptionTopicData.LastPositionUnackedInQueue = newUnackMessages.Last().Id;
                }

                if (newUnackMessages.Count() == 0)
                {
                    isMemoryLoaded = false;
                    subscriptionTopicData.DoesUnackedMessagesExists = false;
                }
            }

            return isMemoryLoaded;
        }

        private async void SubscriptionTopicData_TriggerLogic(object sender, string subscriptionId)
        {
            await StoreCurrentPositionAsync(subscriptionId);
        }

        private async void SubscriptionTopicData_ReadMessagesFromStorage(object sender, string subscriptionId)
        {
            // check if new messages arrived in memory.
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            if (subscriptionTopicData.Subscription.SubscriptionMode == SubscriptionMode.NonResilient && subscriptionTopicData.DoesUnackedMessagesExists == true)
                return;

            if (subscriptionTopicData.IsConsuming == false)
            {
                if (LoadNext100MessagesInMemory(subscriptionId) == true)
                {
                    if (subscriptionTopicData.Subscription.SubscriptionMode == SubscriptionMode.Resilient)
                    {
                        await SendFirstMessage(subscriptionId, subscriptionTopicData.CurrentPosition.ReadLedgerPosition, subscriptionTopicData.CurrentPosition.ReadEntryPosition);
                    }
                    else
                    {
                        // non_resilient mode
                        await SendAllMessages(subscriptionId);
                    }
                }
            }
        }

        public bool CheckIfUnackedMessagesExists(string subscriptionId, long ledgerId, long entryId)
        {
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            bool isRemoved = subscriptionTopicData.TemporaryUnackedMessageIds.TryRemove($"{ledgerId}:{entryId}", out _);
            if (isRemoved == true)
                return true;

            return isRemoved;
        }

        private bool CheckUnackedMessagesIfExists(string subscriptionId)
        {
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];
            using (var unackedContext = new MessageAcknowledgementContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName))
            {
                unackedContext.ChangeTracker.AutoDetectChangesEnabled = false;
                unackedContext.Database.EnsureCreated();

                if (unackedContext.UnacknowledgedMessages.Count() == 0)
                    return false;

                return true;
            }
        }

        private void ReleaseMemory(string subscriptionId)
        {
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            if (subscriptionTopicData.TemporaryMessages.Count == 0)
            {
                GC.Collect(2, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            }
        }

    }
}
