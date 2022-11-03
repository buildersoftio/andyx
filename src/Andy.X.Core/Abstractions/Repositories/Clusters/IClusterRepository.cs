using Buildersoft.Andy.X.Model.Clusters;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters
{
    public interface IClusterRepository
    {
        void ConnectNode(string nodeId);
        void DisconnectNode(string nodeId);
        List<ReplicaShardConnection> GetReplicaShardConnections();
        ReplicaShardConnection GetMainReplicaConnection(string nodeId);
        ReplicaShardConnection GetMainReplicaConnectionByIndex(int index);
        Cluster GetCluster();
        Shard NewShard();
        bool AddReplicaInLastShard(Replica replica);

        bool AddReplicaConnectionToShard(string nodeId, string nodeConnectionId);
        bool RemoveReplicaConnectionFromShard(string nodeId);

        Shard GetCurrentShard();
        Replica GetCurrentReplica();

        Replica GetReplica(string nodeId);
        Shard GetShard(string nodeId);

        bool ChangeClusterStatus(ClusterStatus clusterStatus);
    }
}
