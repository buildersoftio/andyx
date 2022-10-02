using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Subscriptions;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.Core.Contexts.Subscriptions;
using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Storages;
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
        private readonly IOrchestratorService _orchestratorService;

        private readonly StorageConfiguration _storageConfiguration;
        private readonly NodeConfiguration _nodeConfiguration;

        private readonly ConcurrentDictionary<string, SubscriptionTopicData> _subscriptionTopicData;

        public OutboundMessageService(
            ILogger<OutboundMessageService> logger,
            ISubscriptionHubRepository subscriptionHubRepository,
            ISubscriptionHubService subscriptionHubService,
            IOrchestratorService orchestratorService,
            StorageConfiguration storageConfiguration,
            NodeConfiguration nodeConfiguration)
        {
            _logger = logger;

            _subscriptionHubRepository = subscriptionHubRepository;
            _subscriptionHubService = subscriptionHubService;
            _orchestratorService = orchestratorService;
            _storageConfiguration = storageConfiguration;
            _nodeConfiguration = nodeConfiguration;

            _subscriptionTopicData = new ConcurrentDictionary<string, SubscriptionTopicData>();
        }

        public Task AddSubscriptionTopicData(SubscriptionTopicData subscriptionTopicData)
        {
            var subscriptionId = ConnectorHelper.GetSubcriptionId(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName);
            var nodeSubscriptionId = ConnectorHelper.GetNodeSubcriptionId(_nodeConfiguration.NodeId, subscriptionId);
            if (_subscriptionTopicData.ContainsKey(subscriptionId) != true)
            {
                using (var subDbContext = new SubscriptionPositionContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName))
                {
                    subscriptionTopicData.CurrentPosition = subDbContext.CurrentPosition.OrderBy(x => x.SubscriptionName).FirstOrDefault();
                    subscriptionTopicData.LastMessageEntryPositionSent = subscriptionTopicData.CurrentPosition.ReadEntryPosition;
                }

                using (var topicStateContext = new TopicEntryPositionContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic))
                {
                    subscriptionTopicData.TopicState = topicStateContext.TopicStates.Find(nodeSubscriptionId);
                    subscriptionTopicData.LastUnackedMessageEntryPositionSent = subscriptionTopicData.TopicState.MarkDeleteEntryPosition;

                    // RetentionBackgrounService, check if any HARD_TTL has remove messages before consuming, if yes skip deleted messages
                    if (subscriptionTopicData.TopicState.MarkDeleteEntryPosition > subscriptionTopicData.CurrentPosition.ReadEntryPosition)
                    {
                        subscriptionTopicData.CurrentPosition.ReadEntryPosition = subscriptionTopicData.TopicState.MarkDeleteEntryPosition;
                        subscriptionTopicData.LastMessageEntryPositionSent = subscriptionTopicData.CurrentPosition.ReadEntryPosition;
                    }
                }

                _subscriptionTopicData.TryAdd(subscriptionId, subscriptionTopicData);

                subscriptionTopicData.StoringCurrentPosition += SubscriptionTopicData_TriggerLogic;
                subscriptionTopicData.ReadMessagesFromStorage += SubscriptionTopicData_ReadMessagesFromStorage;
            }

            subscriptionTopicData = _subscriptionTopicData[subscriptionId];
            subscriptionTopicData.StartService();

            return Task.CompletedTask;
        }


        public Task LoadSubscriptionTopicDataInMemory(SubscriptionTopicData subscriptionTopicData)
        {
            var subscriptionId = ConnectorHelper.GetSubcriptionId(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName);
            var nodeSubscriptionId = ConnectorHelper.GetNodeSubcriptionId(_nodeConfiguration.NodeId, subscriptionId);

            if (_subscriptionTopicData.ContainsKey(subscriptionId) != true)
            {
                using (var subDbContext = new SubscriptionPositionContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName))
                {
                    subscriptionTopicData.CurrentPosition = subDbContext.CurrentPosition.OrderBy(x => x.SubscriptionName).FirstOrDefault();
                    subscriptionTopicData.LastMessageEntryPositionSent = subscriptionTopicData.CurrentPosition.ReadEntryPosition;
                }

                using (var topicStateContext = new TopicEntryPositionContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic))
                {
                    subscriptionTopicData.TopicState = topicStateContext.TopicStates.Find(nodeSubscriptionId);
                    subscriptionTopicData.LastUnackedMessageEntryPositionSent = subscriptionTopicData.TopicState.MarkDeleteEntryPosition;

                    // RetentionBackgrounService, check if any HARD_TTL has remove messages before consuming, if yes skip deleted messages
                    if (subscriptionTopicData.TopicState.MarkDeleteEntryPosition > subscriptionTopicData.CurrentPosition.ReadEntryPosition)
                    {
                        subscriptionTopicData.CurrentPosition.ReadEntryPosition = subscriptionTopicData.TopicState.MarkDeleteEntryPosition;
                        subscriptionTopicData.LastMessageEntryPositionSent = subscriptionTopicData.CurrentPosition.ReadEntryPosition;
                    }
                }

                _subscriptionTopicData.TryAdd(subscriptionId, subscriptionTopicData);

                subscriptionTopicData.StoringCurrentPosition += SubscriptionTopicData_TriggerLogic;
                subscriptionTopicData.ReadMessagesFromStorage += SubscriptionTopicData_ReadMessagesFromStorage;
            }

            return Task.CompletedTask;
        }

        public async Task SendFirstMessage(string subscriptionId, long currentEntryId)
        {
            var topicDataService = _orchestratorService.GetTopicDataService(subscriptionId.GetTopicKeyFromSubcriptionId());
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];

            // get the message from the service.
            var isFirstMessageExists = topicDataService.TryGetNext(currentEntryId, out Message message);
            if (isFirstMessageExists == true)
            {
                subscriptionTopic.SetConsumingFlag();

                var firstMessageDetails = message;

                // if we sent the first message we should not update the message.
                await UpdateCurrentPosition(subscriptionId, currentEntryId);

                await _subscriptionHubService.TransmitMessage(subscriptionTopic.Subscription.Tenant,
                    subscriptionTopic.Subscription.Product,
                    subscriptionTopic.Subscription.Component,
                    subscriptionTopic.Subscription.Topic,
                    subscriptionTopic.Subscription.SubscriptionName,
                    firstMessageDetails);
            }
        }

        public async Task SendNextMessage(string subscriptionId, long currentEntryId)
        {
            var topicDataService = _orchestratorService.GetTopicDataService(subscriptionId.GetTopicKeyFromSubcriptionId());
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];

            // ack previous mesasge
            await UpdateCurrentPosition(subscriptionId, currentEntryId);

            //var isNextMessageDequeued = subscriptionTopic.TemporaryMessageQueue.TryDequeue(out string nextMessage, out DateTimeOffset priority);
            var isNextMessageExists = topicDataService.TryGetNext(currentEntryId, out Message nextMessage);
            if (isNextMessageExists == true)
            {
                var nextMessageDetails = nextMessage;

                await _subscriptionHubService.TransmitMessage(subscriptionTopic.Subscription.Tenant,
                    subscriptionTopic.Subscription.Product,
                    subscriptionTopic.Subscription.Component,
                    subscriptionTopic.Subscription.Topic,
                    subscriptionTopic.Subscription.SubscriptionName,
                    nextMessageDetails);
            }
            else
            {
                subscriptionTopic.UnsetConsumingFlag();
            }

            // we dont need to check in memory anymore. for now we are commenting the code.
            // check if messages are 50% in the queue.
            //if (subscriptionTopic.TemporaryMessageQueue.Count == _storageConfiguration.NumberOfMessagesInMemoryToRequest)
            //    LoadNext100MessagesInMemory(subscriptionId);
        }

        public async Task SendSameMessage(string subscriptionId, long currentEntryId)
        {
            var topicDataService = _orchestratorService.GetTopicDataService(subscriptionId.GetTopicKeyFromSubcriptionId());
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];

            await _subscriptionHubService.TransmitMessage(subscriptionTopic.Subscription.Tenant,
                subscriptionTopic.Subscription.Product,
                subscriptionTopic.Subscription.Component,
                subscriptionTopic.Subscription.Topic,
                subscriptionTopic.Subscription.SubscriptionName,
                topicDataService.Get(currentEntryId));
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

                // return LastEntryPositionSent when the ReadEntryPosition is left.
                subscriptionTopic.LastMessageEntryPositionSent = subscriptionTopic.CurrentPosition.ReadEntryPosition;

                ReleaseMemory();
            }

            return Task.CompletedTask;
        }

        public Task StoreCurrentPositionAsync(string subscriptionId)
        {
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];
            var nodeSubscriptionId = ConnectorHelper.GetNodeSubcriptionId(_nodeConfiguration.NodeId, subscriptionId);


            using (var subDbContext = new SubscriptionPositionContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic, subscriptionTopicData.Subscription.SubscriptionName))
            {
                var subPositon = subDbContext.CurrentPosition.OrderBy(x => x.SubscriptionName).FirstOrDefault();

                if (subPositon.ReadEntryPosition != subscriptionTopicData.CurrentPosition.ReadEntryPosition)
                {
                    subPositon.ReadEntryPosition = subscriptionTopicData.CurrentPosition.ReadEntryPosition;
                    subPositon.UpdatedDate = DateTimeOffset.Now;

                    subDbContext.CurrentPosition.Update(subPositon);
                    subDbContext.SaveChanges();
                }
            }

            // Unacknoledged Position for subscription.
            using (var topicStateContext = new TopicEntryPositionContext(subscriptionTopicData.Subscription.Tenant, subscriptionTopicData.Subscription.Product, subscriptionTopicData.Subscription.Component, subscriptionTopicData.Subscription.Topic))
            {
                var state = topicStateContext.TopicStates.Find(nodeSubscriptionId);
                if (state.CurrentEntry != subscriptionTopicData.TopicState.CurrentEntry ||
                   state.MarkDeleteEntryPosition != subscriptionTopicData.TopicState.MarkDeleteEntryPosition)
                {
                    state.CurrentEntry = subscriptionTopicData.TopicState.CurrentEntry;
                    state.MarkDeleteEntryPosition = subscriptionTopicData.TopicState.MarkDeleteEntryPosition;

                    state.UpdatedDate = DateTimeOffset.Now;

                    topicStateContext.TopicStates.Update(state);
                    topicStateContext.SaveChanges();
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
                    if (subTopic.Subscription.SubscriptionMode == SubscriptionMode.Resilient)
                    {
                        Task.Run(async () => await SendFirstMessage(subscription.Key, subTopic.CurrentPosition.ReadEntryPosition));
                    }
                    else
                    {
                        Task.Run(async () => await SendAllMessages(subscription.Key));
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task UpdateCurrentPosition(string subscriptionId, long currentEntryId)
        {
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            subscriptionTopicData.CurrentPosition.ReadEntryPosition = currentEntryId;

            return Task.CompletedTask;
        }

        public async Task SendAllMessages(string subscriptionId)
        {
            var topicDataService = _orchestratorService.GetTopicDataService(subscriptionId.GetTopicKeyFromSubcriptionId());
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];
            subscriptionTopic.SetConsumingFlag();

            long startEntry = subscriptionTopic.LastMessageEntryPositionSent;

            while (topicDataService.TryGetNext(startEntry, out Message nextMessage))
            {
                if (subscriptionTopic.IsConsuming == false)
                    break;

                var nextMessageDetails = nextMessage;

                await _subscriptionHubService.TransmitMessage(subscriptionTopic.Subscription.Tenant,
                    subscriptionTopic.Subscription.Product,
                    subscriptionTopic.Subscription.Component,
                    subscriptionTopic.Subscription.Topic,
                    subscriptionTopic.Subscription.SubscriptionName,
                    nextMessageDetails);

                startEntry = startEntry + 1;
            }

            subscriptionTopic.LastMessageEntryPositionSent = startEntry;
            subscriptionTopic.UnsetConsumingFlag();
        }

        private async Task SendAllUnackedMessages(string subscriptionId)
        {
            var subscriptionTopic = _subscriptionTopicData[subscriptionId];
            subscriptionTopic.SetConsumingFlag();

            var topicDataService = _orchestratorService.GetTopicDataService(subscriptionId.GetTopicKeyFromSubcriptionId());
            var subscriptionDataService = _orchestratorService.GetSubscriptionUnackedDataService(subscriptionId);

            var unackedStartEntry = subscriptionTopic.LastUnackedMessageEntryPositionSent;
            while (subscriptionDataService.TryGetNext(unackedStartEntry, out UnacknowledgedMessage unacknowledgedMessage))
            {
                var messageDetails = topicDataService.Get(unacknowledgedMessage.MessageEntry);
                if (messageDetails != null)
                {
                    subscriptionTopic.TemporaryUnackedMessageIds.TryAdd(unacknowledgedMessage.MessageEntry.ToString(), unackedStartEntry);

                    await _subscriptionHubService.TransmitMessage(subscriptionTopic.Subscription.Tenant,
                       subscriptionTopic.Subscription.Product,
                       subscriptionTopic.Subscription.Component,
                       subscriptionTopic.Subscription.Topic,
                       subscriptionTopic.Subscription.SubscriptionName,
                       messageDetails);
                }

                unackedStartEntry = unackedStartEntry + 1;
            }

            subscriptionTopic.LastUnackedMessageEntryPositionSent = unackedStartEntry;
            subscriptionTopic.UnsetConsumingFlag();
        }

        private async void SubscriptionTopicData_TriggerLogic(object sender, string subscriptionId)
        {
            await StoreCurrentPositionAsync(subscriptionId);
        }

        private async Task<bool> SubscriptionTopicData_ReadMessagesFromStorage(object sender, string subscriptionId)
        {
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];
            try
            {
                //_logger.LogInformation($"checking IsConsuming={subscriptionTopicData.IsConsuming}");

                if (subscriptionTopicData.IsConsuming != true)
                {
                    if (subscriptionTopicData.Subscription.SubscriptionMode == SubscriptionMode.Resilient)
                    {
                        await SendFirstMessage(subscriptionId, subscriptionTopicData.CurrentPosition.ReadEntryPosition);
                    }
                    else
                    {
                        await SendAllMessages(subscriptionId);
                        await SendAllUnackedMessages(subscriptionId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"An error accured, couldn't connect to topic store. Andy X is trying to connect to it, details={ex.Message}.");
            }

            if (_subscriptionHubRepository.GetSubscriptionById(subscriptionId).ConsumersConnected.Count == 0)
                return false;

            // stop the timer if the consumer/s is/are consuming
            if (subscriptionTopicData.IsConsuming == true)
                return false;

            return true;
        }

        public bool CheckIfUnackedMessagesExists(string subscriptionId, long entryId)
        {
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];

            bool isRemoved = subscriptionTopicData.TemporaryUnackedMessageIds.TryRemove(entryId.ToString(), out _);
            if (isRemoved == true)
            {
                return true;
            }

            return isRemoved;
        }

        public void DeleteEntryOfUnackedMessages(string subscriptionId)
        {
            var subscriptionTopicData = _subscriptionTopicData[subscriptionId];
            subscriptionTopicData.TopicState.MarkDeleteEntryPosition++;
        }

        private void ReleaseMemory()
        {
            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }

        public SubscriptionTopicData GetSubscriptionDataConnector(string subscriptionId)
        {
            if (_subscriptionTopicData.ContainsKey(subscriptionId) is false)
                return null;

            return _subscriptionTopicData[subscriptionId];
        }
    }
}
