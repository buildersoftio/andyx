using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Subscriptions;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.Core.Contexts.Subscriptions;
using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Storages;
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
                    subscriptionTopicData.LastEntryPositionInQueue = subscriptionTopicData.CurrentPosition.ReadEntryPosition - 1;
                }

                _subscriptionTopicData.TryAdd(subscriptionId, subscriptionTopicData);
                subscriptionTopicData.StoringCurrentPosition += SubscriptionTopicData_TriggerLogic;
                subscriptionTopicData.ReadMessagesFromStorage += SubscriptionTopicData_ReadMessagesFromStorage;
            }
            subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            // if sub type is Unique.
            // load 100 rows into memory
            // In case of shared subscription, if there exists a consumer conencted
            if (subscriptionTopicData.IsConsuming != true)
            {
                LoadNext100MessagesInMemory(subscriptionId);
                await SendFirstMessage(subscriptionId, subscriptionTopicData.CurrentPosition.ReadLedgerPosition, subscriptionTopicData.CurrentPosition.ReadEntryPosition);
            }

            subscriptionTopicData.StartService();

            return;
        }

        public async Task SendFirstMessage(string subscriptionId, long currentLedgerId, long currentEntryId)
        {
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];

            //TODO: try to PEEK the first message from the queue and check if the message already exists, or if is different from the currentLedger and currentEntryId

            var isFirstMessageDequeued = subscriptionTopic.TemporaryMessageQueue.TryDequeue(out string firstMessage, out DateTimeOffset priority);
            if (isFirstMessageDequeued == true)
            {
                subscriptionTopic.SetConsumingFlag();

                var message = subscriptionTopic.TemporaryMessages[firstMessage];
                subscriptionTopic.CurrentPosition.ReadLedgerPosition = message.LedgerId;
                subscriptionTopic.CurrentPosition.ReadEntryPosition = message.Id;

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

                subscriptionTopic.CurrentPosition.ReadLedgerPosition = message.LedgerId;
                subscriptionTopic.CurrentPosition.ReadEntryPosition = message.Id;

                // delete message from memory
                subscriptionTopic.TemporaryMessages.TryRemove($"{currentLedgerId}:{currentEntryId}", out Message deletedMessage);

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
                else
                {
                    subscriptionTopic.CurrentPosition.ReadEntryPosition = subscriptionTopic.CurrentPosition.ReadEntryPosition + 1;
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

        public async Task TriggerSubscriptionsByProducer(string tenant, string product, string component, string topic)
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
                        await SendFirstMessage(subscription.Key, subTopic.CurrentPosition.ReadLedgerPosition, subTopic.CurrentPosition.ReadEntryPosition);
                    }
                }
            }
        }

        private bool LoadNext100MessagesInMemory(string subscriptionId)
        {
            bool isMemoryLoaded = true;
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            using (var storageContext = new StorageContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.LastLedgerPositionInQueue))
            {

                var newMessages = storageContext.Messages.Where(x => x.Id > subscriptionTopicData.LastEntryPositionInQueue).Take(100);

                foreach (var msg in newMessages)
                {
                    subscriptionTopicData.TemporaryMessageQueue.TryEnqueue($"{msg.LedgerId}:{msg.Id}", msg.SentDate);
                    subscriptionTopicData.TemporaryMessages.TryAdd($"{msg.LedgerId}:{msg.Id}", msg);

                    subscriptionTopicData.LastLedgerPositionInQueue = msg.LedgerId;
                    subscriptionTopicData.LastEntryPositionInQueue = msg.Id;
                }

                if (newMessages.Count() > 0)
                {
                    if (newMessages.OrderBy(x => x.Id).Last().Id == _storageConfiguration.LedgerSize)
                    {
                        // prepare ledger change for the next
                        subscriptionTopicData.LastLedgerPositionInQueue = subscriptionTopicData.LastLedgerPositionInQueue + 1;
                        subscriptionTopicData.LastEntryPositionInQueue = 0;
                    }
                }
            }

            if (subscriptionTopicData.TemporaryMessageQueue.Count == 0)
                isMemoryLoaded = false;

            return isMemoryLoaded;
        }

        private async void SubscriptionTopicData_TriggerLogic(object sender, string subscriptionId)
        {
            await StoreCurrentPositionAsync(subscriptionId);
        }

        private async void SubscriptionTopicData_ReadMessagesFromStorage(object sender, string subscriptionId)
        {
            //  check if new messages arrived in memory.
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];
            if (subscriptionTopicData.IsConsuming == false)
            {
                if (LoadNext100MessagesInMemory(subscriptionId) == true)
                {
                    await SendFirstMessage(subscriptionId, subscriptionTopicData.CurrentPosition.ReadLedgerPosition, subscriptionTopicData.CurrentPosition.ReadEntryPosition);
                }
            }
        }
    }
}
