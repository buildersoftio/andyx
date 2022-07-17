using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.Core.Mappers;
using Buildersoft.Andy.X.Core.Services.Inbound.Connectors;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Services.Inbound
{
    public class InboundMessageService : IInboundMessageService
    {
        private readonly ILogger<InboundMessageService> _logger;
        private readonly ThreadsConfiguration _threadsConfiguration;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ConcurrentDictionary<string, TopicDataConnector> _topicConnectors;
        private readonly IOutboundMessageService _outboundMessageService;
        private readonly StorageConfiguration _storageConfiguration;
        private readonly NodeConfiguration _nodeConfiguration;

        private readonly ITenantService _tenantRepository;

        public InboundMessageService(ILogger<InboundMessageService> logger,
            ThreadsConfiguration threadsConfiguration,
            ITenantService tenantRepository,
            IOrchestratorService orchestratorService,
            IOutboundMessageService outboundMessageService,
            StorageConfiguration storageConfiguration,
            NodeConfiguration nodeConfiguration)
        {
            _logger = logger;

            _threadsConfiguration = threadsConfiguration;
            _tenantRepository = tenantRepository;

            _orchestratorService = orchestratorService;
            _outboundMessageService = outboundMessageService;
            _storageConfiguration = storageConfiguration;
            _nodeConfiguration = nodeConfiguration;
            _topicConnectors = new ConcurrentDictionary<string, TopicDataConnector>();
        }

        public void AcceptMessage(Message message)
        {
            var topic = _tenantRepository.GetTopic(message.Tenant, message.Product, message.Component, message.Topic);
            string topicKey = ConnectorHelper.GetTopicConnectorKey(message.Tenant, message.Product, message.Component, message.Topic);

            TryCreateTopicConnector(topicKey);
            _topicConnectors[topicKey].MessagesBuffer.Enqueue(message.Map(_nodeConfiguration.NodeId, topic.TopicStates.LatestEntryId++));

            InitializeInboundMessageProcessor(topicKey);
        }



        #region Store Message Region
        private void InitializeInboundMessageProcessor(string connectorKey)
        {
            if (_topicConnectors[connectorKey].MessageStoreThreadingPool.AreThreadsRunning != true)
            {
                _topicConnectors[connectorKey].MessageStoreThreadingPool.AreThreadsRunning = true;

                // initialize threads.
                InitializeInboundMessageProcessorThreads(connectorKey);
            }
        }

        private void InitializeInboundMessageProcessorThreads(string connectorKey)
        {
            foreach (var thread in _topicConnectors[connectorKey].MessageStoreThreadingPool.Threads)
            {
                if (thread.Value.IsThreadWorking != true)
                {
                    try
                    {
                        thread.Value.IsThreadWorking = true;
                        thread.Value.Task = Task.Run(() => InboundMessagingProcessor(connectorKey, thread.Key));
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"Inbound message processor thread '{thread.Key}' failed to restart");
                    }
                }
            }
        }

        private void InboundMessagingProcessor(string topicKey, Guid threadId)
        {
            while (_topicConnectors[topicKey].MessagesBuffer.TryDequeue(out Model.Entities.Storages.Message message))
            {
                try
                {
                    _orchestratorService.GetTopicDataService(topicKey).Put(message.Entry.ToString(), message);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error accured, read details {ex.Message}");
                }
            }

            _topicConnectors[topicKey].MessageStoreThreadingPool.Threads[threadId].IsThreadWorking = false;
        }

        private bool TryCreateTopicConnector(string topicKey)
        {
            if (_topicConnectors.ContainsKey(topicKey))
                return true;

            var msgTopicConnector = new TopicDataConnector(topicKey, _threadsConfiguration.MaxNumber, _storageConfiguration.InboundMemoryReleaseInMilliseconds, _storageConfiguration.InboundFlushCurrentEntryPositionInMilliseconds);
            msgTopicConnector.StoringCurrentEntryPosition += MsgTopicConnector_StoringCurrentEntryPosition;

            return _topicConnectors.TryAdd(topicKey, msgTopicConnector);
        }

        private void MsgTopicConnector_StoringCurrentEntryPosition(object sender, string topicKey)
        {
            (string tenant, string product, string component, string topic) = topicKey.GetDetailsFromTopicKey();
            var topicDetails = _tenantRepository.GetTopic(tenant, product, component, topic);

            using (var topicStateContext = new TopicStateContext(tenant, product, component, topic))
            {
                var currentData = topicStateContext.TopicStates.Find("DEFAULT");
                currentData.CurrentEntry = topicDetails.TopicStates.LatestEntryId;
                currentData.MarkDeleteEntryPosition = topicDetails.TopicStates.MarkDeleteEntryPosition;
                currentData.UpdatedDate = DateTimeOffset.Now;

                topicStateContext.TopicStates.Update(currentData);
                topicStateContext.SaveChanges();
            }
        }

        #endregion

        public void AcceptUnacknowledgedMessage(string tenant, string product, string component, string topic, string subscription, MessageAcknowledgedDetails messageAcknowledgement)
        {
            var subscriptionId = ConnectorHelper.GetSubcriptionId(tenant, product, component, topic, subscription);

            // Store here only if this entry message has come from this node.
            if (messageAcknowledgement.NodeId == _nodeConfiguration.NodeId)
            {
                var unacknowledgedMessage = new Model.Entities.Storages.UnacknowledgedMessage() { MessageEntry = messageAcknowledgement.EntryId };
                var topicStateOfSubscription = _outboundMessageService.GetSubscriptionDataConnector(subscriptionId).TopicState;
                topicStateOfSubscription.CurrentEntryOfUnacknowledgedMessage = topicStateOfSubscription.CurrentEntryOfUnacknowledgedMessage + 1;
                _orchestratorService.GetSubscriptionUnackedDataService(subscriptionId).Put(topicStateOfSubscription.CurrentEntryOfUnacknowledgedMessage.ToString(), unacknowledgedMessage);
            }
            else
            {
                // TODO: Implement Clustering.
            }
        }
    }
}
