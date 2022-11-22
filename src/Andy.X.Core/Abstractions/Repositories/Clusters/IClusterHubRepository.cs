using Buildersoft.Andy.X.Model.Clusters;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters
{
    public interface IClusterHubRepository
    {
        bool AddNodeClient(string connectionId, NodeClient clusterClient);
        bool RemoveNodeClient(string connectionId);

        NodeClient GetNodeClientById(string connectionId);
        NodeClient GetNodeClientByNodeId(string nodeId);
        string GetNodeConnectionIdByNodeId(string nodeId);
    }
}
