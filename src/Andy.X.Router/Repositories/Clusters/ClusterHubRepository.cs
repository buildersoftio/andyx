using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Model.Clusters;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq;

namespace Buildersoft.Andy.X.Router.Repositories.Clusters
{
    public class ClusterHubRepository : IClusterHubRepository
    {
        private readonly ILogger<ClusterHubRepository> _logger;
        private readonly IClusterRepository _clusterRepository;
        private readonly ConcurrentDictionary<string, NodeClient> _nodesInCluster;

        public ClusterHubRepository(ILogger<ClusterHubRepository> logger, IClusterRepository clusterRepository)
        {
            _logger = logger;
            _clusterRepository = clusterRepository;
            _nodesInCluster = new ConcurrentDictionary<string, NodeClient>();
        }

        public bool AddNodeClient(string connectionId, NodeClient nodeClient)
        {
            // update the replica to connected.
            if (_nodesInCluster.TryAdd(connectionId, nodeClient) == true)
            {
                var replicaConnected = _clusterRepository.GetReplica(nodeClient.NodeId);
                replicaConnected.IsConnected = true;

                return true;
            }
            return false;
        }

        public NodeClient GetNodeClientById(string connectionId)
        {
            if (_nodesInCluster.ContainsKey(connectionId))
                return _nodesInCluster[connectionId];

            return null;
        }

        public NodeClient GetNodeClientByNodeId(string nodeId)
        {
            var nodeClient = _nodesInCluster.Values.Where(n => n.NodeId == nodeId).FirstOrDefault();

            if (nodeClient == null)
                return null;


            return nodeClient;
        }

        public string GetNodeConnectionIdByNodeId(string nodeId)
        {
            return _nodesInCluster.Where(k => k.Value.NodeId == nodeId).FirstOrDefault().Key;
        }

        public bool RemoveNodeClient(string connectionId)
        {
            if (_nodesInCluster.ContainsKey(connectionId) != true)
                return true;

            if (_nodesInCluster.TryRemove(connectionId, out NodeClient deletedNode) == true)
            {
                var replicaConnected = _clusterRepository.GetReplica(deletedNode.NodeId);
                replicaConnected.IsConnected = false;

                return true;
            }

            return false;
        }
    }
}
