using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services;
using Buildersoft.Andy.X.Core.Contexts.Clusters;
using Buildersoft.Andy.X.IO.Services;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly ITenantStateService _tenantService;
        private readonly ITenantFactory _tenantFactory;
        private readonly StorageConfiguration _storageConfiguration;
        private readonly IInboundMessageService _inboundMessageService;
        private readonly ICoreService _coreService;

        private readonly ConcurrentDictionary<string, Task<NodeClusterEventService>> _nodesClientServices;

        public ClusterService(ILoggerFactory loggerFactory,
            IClusterRepository clusterRepository,
            ClusterConfiguration clusterConfiguration,
            NodeConfiguration nodeConfiguration,
            IProducerHubRepository producerHubRepository,
            IProducerFactory producerFactory,
            ISubscriptionHubRepository subscriptionHubRepository,
            IConsumerFactory consumerFactory,
            ISubscriptionFactory subscriptionFactory,
            ITenantStateService tenantService,
            ITenantFactory tenantFactory,
            StorageConfiguration storageConfiguration,
            IInboundMessageService inboundMessageService,
            ICoreService coreService)
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
            _tenantService = tenantService;
            _tenantFactory = tenantFactory;
            _storageConfiguration = storageConfiguration;
            _inboundMessageService = inboundMessageService;
            _coreService = coreService;


            _nodesClientServices = new ConcurrentDictionary<string, Task<NodeClusterEventService>>();

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
                    if (replica.NodeId != _nodeConfiguration.NodeId)
                    {
                        var key = replica.NodeId;

                        _logger.LogInformation($"Initiating cluster connection to node '{replica.NodeId}'");

                        // creating directories for cluster
                        ClusterIOService.TryCreateClusterNodeDirectory(key);

                        // try create db context for each shard.
                        TryCreateClusterChangeLogState(key);

                        var nodeClusterEventServiceTask = new Task<NodeClusterEventService>(() =>
                        {
                            return new NodeClusterEventService(_loggerFactory.CreateLogger<NodeClusterEventService>(),
                                 replica,
                                 clusterConfiguration,
                                  _producerHubRepository,
                                  _producerFactory,
                                  _subscriptionHubRepository,
                                  _tenantService,
                                  _tenantFactory,
                                  _nodeConfiguration,
                                  _consumerFactory,
                                  _subscriptionFactory,
                                  _inboundMessageService,
                                  _coreService);
                        });

                        _nodesClientServices.TryAdd(key, nodeClusterEventServiceTask);
                        nodeClusterEventServiceTask.Start();
                    }
                }
            }

            var currentReplica = _clusterRepository.GetCurrentReplica();
            if (currentReplica == null)
            {
                _logger.LogError($"This node with id {_nodeConfiguration.NodeId} is not configured in the cluster");
                _logger.LogError("Andy X is shutting down unexpectedly");
                throw new Exception($"This node with id {_nodeConfiguration.NodeId} is not configured in the cluster");
            }

            // initialize Cluster Async Connector
            var activeMainShards = _clusterRepository.GetReplicaShardConnections();
            foreach (var replica in activeMainShards)
            {
                if (replica.NodeId != _nodeConfiguration.NodeId)
                    _inboundMessageService.TryCreateClusterAsyncConnector(replica.NodeId, activeMainShards.Count());
            }

            if (activeMainShards.Count() > 1)
            {
                _inboundMessageService.NodeIsRunningInsideACluster();
            }

            _clusterRepository.ChangeClusterStatus(ClusterStatus.Online);
        }

        private void TryCreateClusterChangeLogState(string key)
        {
            var replicaInShardConnection = _clusterRepository.GetMainReplicaConnection(key);
            using (var clusterStateContext = new ClusterEntryPositionContext(key))
            {
                clusterStateContext.Database.EnsureCreated();

                var currentData = clusterStateContext.NodeEntryStates.Find(key);
                if (currentData == null)
                {
                    currentData = new Model.Entities.Storages.TopicEntryPosition()
                    {
                        Id = key,
                        CurrentEntry = 1,
                        MarkDeleteEntryPosition = 1,

                        CreateDate = DateTimeOffset.Now
                    };

                    clusterStateContext.NodeEntryStates.Add(currentData);
                    clusterStateContext.SaveChanges();
                }

                if (replicaInShardConnection != null)
                {
                    replicaInShardConnection.NodeEntryState = currentData;
                }
            }
        }
    }
}
