using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Buildersoft.Andy.X.Core.Services.Outbound
{
    public class OutboundClusterMessageService
    {
        private readonly ILogger<OutboundClusterMessageService> _logger;
        private readonly IClusterRepository _clusterRepository;
        private readonly IClusterHubService _clusterHubService;
        private readonly IOrchestratorService _orchestratorService;
        private readonly StorageConfiguration _storageConfiguration;
        private readonly NodeConfiguration _nodeConfiguration;

        private readonly ConcurrentDictionary<string, ClusterTemporaryReaderConnector> _clusterTempReaders;

        public OutboundClusterMessageService(ILogger<OutboundClusterMessageService> logger,
            IClusterRepository clusterRepository,
            IClusterHubService clusterHubService,
            IOrchestratorService orchestratorService,
            StorageConfiguration storageConfiguration,
            NodeConfiguration nodeConfiguration)
        {
            _logger = logger;
            _clusterRepository = clusterRepository;
            _clusterHubService = clusterHubService;
            _orchestratorService = orchestratorService;
            _storageConfiguration = storageConfiguration;
            _nodeConfiguration = nodeConfiguration;

            _clusterTempReaders = new ConcurrentDictionary<string, ClusterTemporaryReaderConnector>();

            InitializeClusterNodeReadServices();
        }

        private void InitializeClusterNodeReadServices()
        {
            // Get nodes connected here.
            var nodesConnected = _clusterRepository.GetReplicaShardConnections()
                .Where(x => x.NodeId != _nodeConfiguration.NodeId);
            if (nodesConnected == null)
                return;

            foreach (var node in nodesConnected)
            {
                var reader = new ClusterTemporaryReaderConnector(node.NodeId, _storageConfiguration.OutboundFlushCurrentEntryPositionInMilliseconds, _storageConfiguration.OutboundBackgroundIntervalReadMessagesInMilliseconds);
                reader.ReadMessagesFromTempStorage += Reader_ReadMessagesFromTempStorage;
                _clusterTempReaders.TryAdd(node.NodeId, reader);
            }
        }

        private void Reader_ReadMessagesFromTempStorage(object sender, string nodeId)
        {
            var nodeReplica = _clusterRepository.GetMainReplicaConnection(nodeId);
            if (nodeReplica.NodeEntryState.CurrentEntry != nodeReplica.NodeEntryState.MarkDeleteEntryPosition)
                SendAllMessages(nodeReplica);

        }

        private void SendAllMessages(ReplicaShardConnection nodeReplica)
        {
            while (nodeReplica.NodeEntryState.CurrentEntry != nodeReplica.NodeEntryState.MarkDeleteEntryPosition)
            {

                if (nodeReplica.NodeConnectionId == "")
                    break;

                var message = _orchestratorService.GetClusterDataService(nodeReplica.NodeId).Get(nodeReplica.NodeEntryState.MarkDeleteEntryPosition);
                if (message == null)
                    break;

                _clusterHubService
                    .DistributeMessage_ToNode(nodeReplica.NodeId, nodeReplica.NodeConnectionId, message);

                _orchestratorService.GetClusterDataService(nodeReplica.NodeId).Delete(nodeReplica.NodeEntryState.MarkDeleteEntryPosition);

                // increes the deleted entry position
                nodeReplica.NodeEntryState.MarkDeleteEntryPosition = nodeReplica.NodeEntryState.MarkDeleteEntryPosition + 1;
            }
        }


        public void StopService(string nodeId)
        {
            if (_clusterTempReaders.ContainsKey(nodeId))
                _clusterTempReaders[nodeId].StopService();
        }

        public void StartService(string nodeId)
        {
            if (_clusterTempReaders.ContainsKey(nodeId))
            {
                _clusterTempReaders[nodeId].StartService();
            }
        }
    }
}
