using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.Core.Mappers;
using Buildersoft.Andy.X.Core.Services.Inbound.Connectors;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Configurations;
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

        private readonly ITenantRepository _tenantRepository;

        public InboundMessageService(ILogger<InboundMessageService> logger,
            ThreadsConfiguration threadsConfiguration,
            ITenantRepository tenantRepository,
            IOrchestratorService orchestratorService,
            IOutboundMessageService outboundMessageService,
            StorageConfiguration storageConfiguration)
        {
            _logger = logger;

            _threadsConfiguration = threadsConfiguration;
            _tenantRepository = tenantRepository;

            _orchestratorService = orchestratorService;
            _outboundMessageService = outboundMessageService;
            _storageConfiguration = storageConfiguration;

            _topicConnectors = new ConcurrentDictionary<string, TopicDataConnector>();
        }

        public void AcceptMessage(Message message)
        {
            var topic = _tenantRepository.GetTopic(message.Tenant, message.Product, message.Component, message.Topic);
            string topicKey = ConnectorHelper.GetTopicConnectorKey(message.Tenant, message.Product, message.Component, message.Topic);

            TryCreateTopicConnector(topicKey);
            _topicConnectors[topicKey].MessagesBuffer.Enqueue(message.Map(topic.TopicStates.LatestEntryId++));

            InitializeInboundMessageProcessor(topicKey);
        }

        public void AcceptUnacknowledgedMessage(MessageAcknowledgementFileContent messageAcknowledgement)
        {
            // TODO: Update Unacked messages with consumption.
            string connectorKey = ConnectorHelper.GetTopicConnectorKey(messageAcknowledgement.Tenant, messageAcknowledgement.Product, messageAcknowledgement.Component, messageAcknowledgement.Topic);
            TryCreateTopicConnector(connectorKey);

            //_topicConnectors[connectorKey].UnacknowledgedMessageBuffer.Enqueue(messageAcknowledgement);

            InitializeInboundUnacknowledgedMessageProcessor(connectorKey);
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

        #region Store Unacked Messages Region
        private void InitializeInboundUnacknowledgedMessageProcessor(string connectorKey)
        {
            //if (_topicConnectors[connectorKey].UnacknowledgedMessageThreadingPool.AreThreadsRunning != true)
            //{
            //    _topicConnectors[connectorKey].UnacknowledgedMessageThreadingPool.AreThreadsRunning = true;

            //    // initialize threads.
            //    InitializeInboundUnacknowledgedMessageProcessorThreads(connectorKey);
            //}
        }
        private void InitializeInboundUnacknowledgedMessageProcessorThreads(string connectorKey)
        {
            //foreach (var thread in _topicConnectors[connectorKey].UnacknowledgedMessageThreadingPool.Threads)
            //{
            //    if (thread.Value.IsThreadWorking != true)
            //    {
            //        try
            //        {
            //            thread.Value.IsThreadWorking = true;
            //            thread.Value.Task = Task.Run(() => InboundUnacknowledgedMessagingProcessor(connectorKey, thread.Key));
            //        }
            //        catch (Exception)
            //        {
            //            _logger.LogError($"Inbound unacknowledged message processor thread '{thread.Key}' failed to restart");
            //        }
            //    }
            //}
        }

        private void InboundUnacknowledgedMessagingProcessor(string connectorKey, Guid threadId)
        {
            //while (_topicConnectors[connectorKey].UnacknowledgedMessageBuffer.TryDequeue(out MessageAcknowledgementFileContent messageAcknowledgement))
            //{
            //    try
            //    {
            //        var msgId = $"ua_{messageAcknowledgement.Subscription}" + Guid.NewGuid();
            //        if (messageAcknowledgement.IsDeleted == true)
            //            msgId = $"del_" + Guid.NewGuid();

            //        MessageIOService.TrySaveInTemp_UnackedMessageBinFile(messageAcknowledgement, msgId);


            //        // TODO: Implement Cluster Syncronization of messages.
            //    }
            //    catch (Exception)
            //    {
            //        // TODO: check this later;
            //    }
            //}

            //_topicConnectors[connectorKey].UnacknowledgedMessageThreadingPool.Threads[threadId].IsThreadWorking = false;

            // check release memory.
            ReleaseMemoryUnacknowledgedMessagingProcessor(connectorKey);
        }
        private void ReleaseMemoryUnacknowledgedMessagingProcessor(string connectorKey)
        {
            //if (_topicConnectors[connectorKey].UnacknowledgedMessageBuffer.Count == 0)
            //{
            //    GC.Collect(2, GCCollectionMode.Forced);
            //    GC.WaitForPendingFinalizers();
            //}
        }

        #endregion
    }
}
