using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;

namespace Buildersoft.Andy.X.Core.Services.Clusters
{
    public class ClusterService : IClusterService
    {
        private readonly ILogger<ClusterService> _logger;
        private readonly IClusterRepository _clusterRepository;
        private readonly ClusterConfiguration _clusterConfiguration;
        private readonly NodeConfiguration _nodeConfiguration;

        public ClusterService(ILogger<ClusterService> logger,
            IClusterRepository clusterRepository,
            ClusterConfiguration clusterConfiguration,
            NodeConfiguration nodeConfiguration)
        {
            _logger = logger;

            _clusterRepository = clusterRepository;
            _clusterConfiguration = clusterConfiguration;
            _nodeConfiguration = nodeConfiguration;

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
