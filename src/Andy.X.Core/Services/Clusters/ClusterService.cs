using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Core.Services.Clusters
{
    public class ClusterService : IClusterService
    {
        private readonly ILogger<ClusterService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IClusterRepository _clusterRepository;
        private readonly ClusterConfiguration _clusterConfiguration;
        private readonly NodeConfiguration _nodeConfiguration;


        private readonly IProducerHubRepository _producerHubRepository;
        private readonly IProducerFactory _producerFactory;
        private readonly ISubscriptionHubRepository _subscriptionHubRepository;
        private readonly IConsumerFactory _consumerFactory;
        private readonly ISubscriptionFactory _subscriptionFactory;


        private readonly ConcurrentDictionary<string, NodeClusterEventService> _nodesClientServices;

        public ClusterService(ILoggerFactory loggerFactory,
            IClusterRepository clusterRepository,
            ClusterConfiguration clusterConfiguration,
            NodeConfiguration nodeConfiguration,
            IProducerHubRepository producerHubRepository,
            IProducerFactory producerFactory,
            ISubscriptionHubRepository subscriptionHubRepository,
            IConsumerFactory consumerFactory,
            ISubscriptionFactory subscriptionFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ClusterService>();

            _clusterRepository = clusterRepository;
            _clusterConfiguration = clusterConfiguration;
            _nodeConfiguration = nodeConfiguration;


            _producerHubRepository = producerHubRepository;
            _producerFactory = producerFactory;
            _subscriptionHubRepository = subscriptionHubRepository;
            _consumerFactory = consumerFactory;
            _subscriptionFactory = subscriptionFactory;

            _nodesClientServices = new ConcurrentDictionary<string, NodeClusterEventService>();

            // loading cluster configurations in-memory of this node.
            LoadClusterConfigurationInMemory(clusterConfiguration);
        }

        public void ChangeClusterStatus(ClusterStatus clusterStatus)
        {
            _clusterRepository.ChangeClusterStatus(clusterStatus);
        }

        public Cluster GetCluster()
        {
            return _clusterRepository.GetCluster();
        }

        public void LoadClusterConfigurationInMemory(ClusterConfiguration clusterConfiguration)
        {
            foreach (var shard in _clusterConfiguration.Shards)
            {
                var newShard = _clusterRepository.NewShard();
                newShard.ReplicaDistributionType = shard.ReplicaDistributionType;

                foreach (var replica in shard.Replicas)
                {
                    _clusterRepository.AddReplicaInLastShard(replica);

                    // Create connection for each node, ignore local node.
                    // TESTING
                    //if (replica.NodeId != _nodeConfiguration.NodeId)
                    //{
                        var key = replica.NodeId;

                        var nodeClusterEventService = new NodeClusterEventService(_loggerFactory.CreateLogger<NodeClusterEventService>(),
                            replica,
                            clusterConfiguration,
                             _producerHubRepository,
                             _producerFactory,
                             _subscriptionHubRepository,
                             _consumerFactory,
                             _subscriptionFactory);

                        _nodesClientServices.TryAdd(key, nodeClusterEventService);
                        // connect the node to cluster.
                        nodeClusterEventService.ConnectAsync();
                    //}
                }
            }

            var currentReplica = _clusterRepository.GetCurrentReplica();
            if (currentReplica == null)
            {
                _logger.LogError($"This node with id {_nodeConfiguration.NodeId} is not configured in the cluster");
                _logger.LogError("Andy X is shutting down unexpectedly");
                throw new System.Exception($"This node with id {_nodeConfiguration.NodeId} is not configured in the cluster");
            }

            _clusterRepository.ChangeClusterStatus(ClusterStatus.Online);
        }
    }
}
