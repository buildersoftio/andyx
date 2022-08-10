using Buildersoft.Andy.X.Model.Clusters;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Clusters
{
    public interface IClusterFactory
    {
        NodeClient CreateNodeClient(string nodeId, string hostName, int shardId, ReplicaTypes replicaType);
    }
}
