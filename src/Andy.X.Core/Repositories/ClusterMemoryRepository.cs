using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Buildersoft.Andy.X.Core.Repositories
{
    public class ClusterMemoryRepository : IClusterRepository
    {
        private readonly ILogger<ClusterMemoryRepository> _logger;
        private readonly NodeConfiguration _nodeConfiguration;

        private readonly Cluster _cluster;

        private Shard _currentShard;
        private Replica _currentReplica;

        private readonly List<ReplicaShardConnection> _mainReplicasAsShards;

        public ClusterMemoryRepository(ILogger<ClusterMemoryRepository> logger,
            NodeConfiguration nodeConfiguration,
            ClusterConfiguration clusterConfiguration)
        {
            _logger = logger;
            _nodeConfiguration = nodeConfiguration;

            _currentShard = null;
            _currentReplica = null;

            _mainReplicasAsShards = new List<ReplicaShardConnection>();

            _cluster = new Cluster()
            {
                Name = clusterConfiguration.Name,
                ShardDistributionType = clusterConfiguration.ShardDistributionType,
                Status = ClusterStatus.Starting
            };
        }

        public bool AddReplicaInLastShard(Replica replica)
        {
            if (GetReplica(replica.NodeId) != null)
                return false;

            var lastShard = _cluster.Shards.Last();

            if (replica.Type == ReplicaTypes.Main)
            {
                var mainReplica = lastShard.Replicas.Where(r => r.Type == ReplicaTypes.Main).FirstOrDefault();
                if (mainReplica != null)
                {
                    _logger.LogError($"There is already a node connected in this shard that is working as main, node_id {mainReplica.NodeId}, host_name {mainReplica.Host}");
                    _logger.LogError("Andy X is shutting down unexpectedly");
                    throw new System.Exception($"There is already a node connected in this shard that is working as main, node_id {mainReplica.NodeId}, host_name {mainReplica.Host}");
                }
                _mainReplicasAsShards.Add(new ReplicaShardConnection() { NodeConnectionId = "", NodeId = replica.NodeId, NodeEntryState = null });
            }

            lastShard.Replicas.Add(replica);

            if (replica.NodeId == _nodeConfiguration.NodeId)
            {
                _currentReplica = replica;
                _currentShard = lastShard;

                replica.IsConnected = true;
                replica.IsLocal = true;
            }

            return true;
        }

        public Shard NewShard()
        {
            var shard = new Shard();

            if (_cluster.Shards.Count > 0)
            {
                var mainReplica = _cluster.Shards[_cluster.Shards.Count - 1].Replicas.Where(r => r.Type == ReplicaTypes.Main).FirstOrDefault();
                if (mainReplica == null)
                {
                    _logger.LogError($"Creating new shard is not allowed, in previews shard is not any main replica.");
                    _logger.LogError("Andy X is shutting down unexpectedly");
                    throw new System.Exception($"Creating new shard is not allowed, in previews shard is not any main replica.");
                }
            }
            _cluster.Shards.Add(shard);
            return shard;
        }

        public Replica GetCurrentReplica()
        {
            return _currentReplica;
        }

        public Shard GetCurrentShard()
        {
            return _currentShard;
        }

        public Replica GetReplica(string nodeId)
        {
            foreach (var shard in _cluster.Shards)
            {
                var replica = shard.Replicas.Where(r => r.NodeId == nodeId).FirstOrDefault();
                if (replica != null)
                    return replica;
            }

            return null;
        }

        public Shard GetShard(string nodeId)
        {
            foreach (var shard in _cluster.Shards)
            {
                var replica = shard.Replicas.Where(r => r.NodeId == nodeId).FirstOrDefault();
                if (replica != null)
                    return shard;
            }

            return null;
        }

        public bool ChangeClusterStatus(ClusterStatus clusterStatus)
        {
            _cluster.Status = clusterStatus;
            return true;
        }

        public Cluster GetCluster()
        {
            return _cluster;
        }

        public bool AddReplicaConnectionToShard(string nodeId, string nodeConnectionId)
        {
            var mainReplica = _mainReplicasAsShards.Where(x => x.NodeId == nodeId).FirstOrDefault();
            if (mainReplica == null)
                return false;

            var replica = GetReplica(nodeId);
            replica.IsConnected = true;

            mainReplica.NodeConnectionId = nodeConnectionId;
            return true;
        }

        public bool RemoveReplicaConnectionFromShard(string nodeId)
        {
            var mainReplica = _mainReplicasAsShards.Where(x => x.NodeId == nodeId).FirstOrDefault();
            if (mainReplica == null)
                return false;

            var replica = GetReplica(nodeId);
            replica.IsConnected = false;

            mainReplica.NodeConnectionId = "";
            return true;
        }

        public List<ReplicaShardConnection> GetReplicaShardConnections()
        {
            return _mainReplicasAsShards;
        }

        public ReplicaShardConnection GetMainReplicaConnection(string nodeId)
        {
            var mainReplica = _mainReplicasAsShards
                .Where(x => x.NodeId == nodeId)
                .FirstOrDefault();

            if (mainReplica == null)
                return null;

            return mainReplica;
        }

        public ReplicaShardConnection GetMainReplicaConnectionByIndex(int index)
        {
            return _mainReplicasAsShards[index];
        }

        public void ConnectNode(string nodeId)
        {
            var node = GetReplica(nodeId);
            if (node != null)
                node.IsConnected = true;

            var areReplicasConnected = _cluster
                .Shards
                .Any(x => x.Replicas
                .Any(r => r.IsConnected != true) == true);

            if (areReplicasConnected != true)
            {
                _cluster.Status = ClusterStatus.Online;
            }
        }

        public void DisconnectNode(string nodeId)
        {
            var node = GetReplica(nodeId);
            if (node != null)
            {
                node.IsConnected = false;
                _cluster.Status = ClusterStatus.PartiallyOnline;
            }
        }
    }
}
