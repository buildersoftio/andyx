using Buildersoft.Andy.X.Core.Abstractions.Factories.Clusters;
using Buildersoft.Andy.X.Model.Clusters;

namespace Buildersoft.Andy.X.Core.Factories.Clusters
{
    public class ClusterFactory : IClusterFactory
    {
        public NodeClient CreateNodeClient(string nodeId, string hostName, int shardId, ReplicaTypes replicaType)
        {
            return new NodeClient()
            {
                NodeId = nodeId,
                HostName = hostName,
                ShardId = shardId,
                ReplicaType = replicaType
            };
        }
    }
}
