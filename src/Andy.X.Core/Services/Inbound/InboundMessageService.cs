using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Repositories;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Contexts.Clusters;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.Core.Mappers;
using Buildersoft.Andy.X.Core.Services.Inbound.Connectors;
using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Entities.Clusters;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Xml.Linq;
using EntityMessage = Buildersoft.Andy.X.Model.Entities.Storages.Message;

namespace Buildersoft.Andy.X.Core.Services.Inbound
{
    public class InboundMessageService : IInboundMessageService
    {
        private readonly ILogger<InboundMessageService> _logger;
        private readonly ThreadsConfiguration _threadsConfiguration;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ConcurrentDictionary<string, DataConnector<EntityMessage>> _topicConnectors;
        private readonly ConcurrentDictionary<string, DataConnector<ClusterChangeLog>> _clusterAsyncConnectors;
        private readonly IOutboundMessageService _outboundMessageService;
        private readonly StorageConfiguration _storageConfiguration;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly IClusterRepository _clusterRepository;
        private readonly IClusterHubService _clusterHubService;

        private readonly ITenantStateRepository _tenantStateRepository;

        private bool isRunningUnderCluster;
        private int shardCount;

        public InboundMessageService(ILogger<InboundMessageService> logger,
            ThreadsConfiguration threadsConfiguration,
            ITenantStateRepository tenantStateService,
            IOrchestratorService orchestratorService,
            IOutboundMessageService outboundMessageService,
            StorageConfiguration storageConfiguration,
            NodeConfiguration nodeConfiguration,
            IClusterRepository clusterRepository,
            IClusterHubService clusterHubService)
        {
            isRunningUnderCluster = false;
            shardCount = 1;

            _logger = logger;

            _threadsConfiguration = threadsConfiguration;
            _tenantStateRepository = tenantStateService;

            _orchestratorService = orchestratorService;
            _outboundMessageService = outboundMessageService;
            _storageConfiguration = storageConfiguration;
            _nodeConfiguration = nodeConfiguration;
            _clusterRepository = clusterRepository;
            _clusterHubService = clusterHubService;
            _topicConnectors = new ConcurrentDictionary<string, DataConnector<EntityMessage>>();
            _clusterAsyncConnectors = new ConcurrentDictionary<string, DataConnector<ClusterChangeLog>>();
        }

        public void AcceptMessage(Message message, string nodeId = "")
        {
            var topic = _tenantStateRepository.GetTopic(message.Tenant, message.Product, message.Component, message.Topic);
            var topicKey = ConnectorHelper.GetTopicConnectorKey(message.Tenant, message.Product, message.Component, message.Topic);

            // check if the system is under clustering...
            if (isRunningUnderCluster == false)
            {
                TryCreateTopicConnector(topicKey, shardCount);
                _topicConnectors[topicKey].MessagesBuffer.Enqueue(message.Map(_nodeConfiguration.NodeId, topic.TopicStates.LatestEntryId++));
                InitializeInboundMessageProcessor(topicKey);
                return;
            }

            // dynamic.
            ReplicaShardConnection node = null;
            if (nodeId == "")
            {
                node = _clusterRepository.GetMainReplicaConnectionByIndex(_topicConnectors[topicKey].GetNextCurrentClusterShardId());
                nodeId = node.NodeId;
            }
            else
            {
                node = _clusterRepository.GetMainReplicaConnection(nodeId);
            }


            if (nodeId == _nodeConfiguration.NodeId)
            {
                TryCreateTopicConnector(topicKey, shardCount);
                _topicConnectors[topicKey].MessagesBuffer.Enqueue(message.Map(_nodeConfiguration.NodeId, topic.TopicStates.LatestEntryId++));

                InitializeInboundMessageProcessor(topicKey);
            }
            else
            {
                // store into cluster file....
                _clusterAsyncConnectors[nodeId].MessagesBuffer.Enqueue(message.Map(node.NodeEntryState.CurrentEntry++));
                InitializeInboundClusterProcessor(nodeId);
            }
        }

        #region Store Message Region for this node and others
        private void InitializeInboundMessageProcessor(string connectorKey)
        {
            if (_topicConnectors[connectorKey].MessageStoreThreadingPool.AreThreadsRunning != true)
            {
                _topicConnectors[connectorKey].MessageStoreThreadingPool.AreThreadsRunning = true;

                // initialize threads.
                InitializeInboundMessageProcessorThreads(connectorKey);
            }
        }

        private void InitializeInboundClusterProcessor(string nodeId)
        {
            if (_clusterAsyncConnectors[nodeId].MessageStoreThreadingPool.AreThreadsRunning != true)
            {
                _clusterAsyncConnectors[nodeId].MessageStoreThreadingPool.AreThreadsRunning = true;

                // initialize threads.
                InitializeInboundClusterProcessorThreads(nodeId);
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

        private void InitializeInboundClusterProcessorThreads(string nodeId)
        {
            foreach (var thread in _clusterAsyncConnectors[nodeId].MessageStoreThreadingPool.Threads)
            {
                if (thread.Value.IsThreadWorking != true)
                {
                    try
                    {
                        thread.Value.IsThreadWorking = true;
                        thread.Value.Task = Task.Run(() => InboundClusterMessagingProcessor(nodeId, thread.Key));
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"Inbound cluster asyunc processor thread '{thread.Key}' failed to restart");
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

        private void InboundClusterMessagingProcessor(string nodeId, Guid threadId)
        {
            var nodeReplica = _clusterRepository.GetMainReplicaConnection(nodeId);
            var sharedDistributionType = _clusterRepository.GetCluster().ShardDistributionType;
            var clusterService = _orchestratorService.GetClusterDataService(nodeId);

            while (_clusterAsyncConnectors[nodeId].MessagesBuffer.TryDequeue(out var message))
            {
                try
                {
                    // Here if the cluster distributed type is ASYNC, store into async
                    if (sharedDistributionType == DistributionTypes.Async)
                    {
                        clusterService.Put(message.Entry.ToString(), message);
                        continue;
                    }

                    // Send directly to the node, if the node is connected.
                    if (nodeReplica.NodeConnectionId != "")
                    {
                        _clusterHubService
                            .DistributeMessage_ToNode(nodeReplica.NodeId, nodeReplica.NodeConnectionId, message);

                        // simulate deletion for the cluster state
                        nodeReplica.NodeEntryState.MarkDeleteEntryPosition = nodeReplica.NodeEntryState.MarkDeleteEntryPosition + 1;

                        continue;
                    }

                    // store for async communication, because the node was not working at that time.
                    clusterService.Put(message.Entry.ToString(), message);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error accured while storing in {nodeId} cluster temp storage or in-memory, read details {ex.Message}");
                }
            }

            _clusterAsyncConnectors[nodeId].MessageStoreThreadingPool.Threads[threadId].IsThreadWorking = false;
        }

        public bool TryCreateTopicConnector(string topicKey, int countNodesConnected)
        {
            if (_topicConnectors.ContainsKey(topicKey))
                return true;

            var msgTopicConnector = new DataConnector<EntityMessage>(topicKey, _threadsConfiguration.MaxNumber, _storageConfiguration.InboundMemoryReleaseInMilliseconds, _storageConfiguration.InboundFlushCurrentEntryPositionInMilliseconds);
            msgTopicConnector.StoringCurrentEntryPosition += MsgTopicConnector_StoringCurrentEntryPosition;

            if (isRunningUnderCluster == true)
                msgTopicConnector.SetClusterShardCount(shardCount);
            else
                msgTopicConnector.SetClusterShardCount(countNodesConnected);

            return _topicConnectors.TryAdd(topicKey, msgTopicConnector);
        }

        public bool TryCreateClusterAsyncConnector(string nodeId, int countNodesConnected)
        {
            if (_clusterAsyncConnectors.ContainsKey(nodeId))
                return true;

            var clusterAsyncConnector = new DataConnector<ClusterChangeLog>(nodeId, _threadsConfiguration.MaxNumber, _storageConfiguration.InboundMemoryReleaseInMilliseconds, _storageConfiguration.InboundFlushCurrentEntryPositionInMilliseconds);
            clusterAsyncConnector.StoringCurrentEntryPosition += ClusterAsyncConnector_StoringCurrentEntryPosition;

            return _clusterAsyncConnectors.TryAdd(nodeId, clusterAsyncConnector);
        }

        private void ClusterAsyncConnector_StoringCurrentEntryPosition(object sender, string nodeId)
        {
            var replica = _clusterRepository.GetMainReplicaConnection(nodeId);
            using (var clusterContext = new ClusterEntryPositionContext(nodeId))
            {
                var currentData = clusterContext.NodeEntryStates.Find(nodeId);

                if (currentData.CurrentEntry != replica.NodeEntryState.CurrentEntry ||
                    currentData.MarkDeleteEntryPosition != replica.NodeEntryState.MarkDeleteEntryPosition)
                {
                    currentData.CurrentEntry = replica.NodeEntryState.CurrentEntry;
                    currentData.MarkDeleteEntryPosition = replica.NodeEntryState.MarkDeleteEntryPosition;
                    currentData.UpdatedDate = DateTimeOffset.Now;

                    clusterContext.NodeEntryStates.Update(currentData);
                    clusterContext.SaveChanges();
                }
            }
        }

        private void MsgTopicConnector_StoringCurrentEntryPosition(object sender, string topicKey)
        {
            (string tenant, string product, string component, string topic) = topicKey.GetDetailsFromTopicKey();
            var topicDetails = _tenantStateRepository.GetTopic(tenant, product, component, topic);

            using (var topicStateContext = new TopicEntryPositionContext(tenant, product, component, topic))
            {
                var currentData = topicStateContext.TopicStates.Find(_nodeConfiguration.NodeId);

                if (currentData.CurrentEntry != topicDetails.TopicStates.LatestEntryId ||
                    currentData.MarkDeleteEntryPosition != topicDetails.TopicStates.MarkDeleteEntryPosition)
                {
                    currentData.CurrentEntry = topicDetails.TopicStates.LatestEntryId;
                    currentData.MarkDeleteEntryPosition = topicDetails.TopicStates.MarkDeleteEntryPosition;
                    currentData.UpdatedDate = DateTimeOffset.Now;

                    topicStateContext.TopicStates.Update(currentData);
                    topicStateContext.SaveChanges();

                    _logger.LogInformation($"Topic topicId={tenant}/{product}/{component}/{topic} Registering to writeEntryPositon {currentData.CurrentEntry}");

                }
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
                topicStateOfSubscription.CurrentEntry = topicStateOfSubscription.CurrentEntry + 1;
                _orchestratorService.GetSubscriptionUnackedDataService(subscriptionId).Put(topicStateOfSubscription.CurrentEntry.ToString(), unacknowledgedMessage);
            }
            else
            {
                // TODO: Implement Clustering.
            }
        }

        public void NodeIsRunningInsideACluster()
        {
            isRunningUnderCluster = true;
            shardCount = _clusterRepository.GetReplicaShardConnections().Count;

            foreach (var topic in _topicConnectors)
            {
                topic.Value.SetClusterShardCount(shardCount);
            }
        }
    }
}
