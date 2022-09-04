using Buildersoft.Andy.X.Model.Clusters;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters
{
    public interface IClusterRepository
    {
        Cluster GetCluster();
        Shard NewShard();
        bool AddReplicaInLastShard(Replica replica);

        bool AddReplicaConnectionToShard(string nodeId, string nodeConnectionId);
        bool RemoveReplicaConnectionToShard(string nodeId);

        Shard GetCurrentShard();
        Replica GetCurrentReplica();

        Replica GetReplica(string nodeId);
        Shard GetShard(string nodeId);

        bool ChangeClusterStatus(ClusterStatus clusterStatus);
    }
}
