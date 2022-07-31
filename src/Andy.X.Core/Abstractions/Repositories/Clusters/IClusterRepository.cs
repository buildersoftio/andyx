using Buildersoft.Andy.X.Model.Clusters;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters
{
    public interface IClusterRepository
    {
        Cluster GetCluster();
        Shard NewShard();
        bool AddReplicaInLastShard(Replica replica);

        Shard GetCurrentShard();
        Replica GetCurrentReplica();

        Replica GetReplica(string nodeId);
        Shard GetShard(string nodeId);

        bool ChangeClusterStatus(ClusterStatus clusterStatus);
    }
}
