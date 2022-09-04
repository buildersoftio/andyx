using Buildersoft.Andy.X.Core.Abstractions.Factories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Model.Clusters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Clusters
{
    //[Authorize]
    public class ClusterHub : Hub<IClusterHub>
    {
        private readonly ILogger<ClusterHub> _logger;
        private readonly IClusterService _clusterService;
        private readonly IClusterHubRepository _clusterHubRepository;
        private readonly IClusterFactory _clusterFactory;
        private readonly IClusterRepository _clusterRepository;

        public ClusterHub(ILogger<ClusterHub> logger,
            IClusterService clusterService,
            IClusterHubRepository clusterHubRepository,
            IClusterFactory clusterFactory,
            IClusterRepository clusterRepository)
        {
            _logger = logger;

            _clusterService = clusterService;
            _clusterHubRepository = clusterHubRepository;
            _clusterFactory = clusterFactory;
            _clusterRepository = clusterRepository;
        }

        public override Task OnConnectedAsync()
        {
            NodeClient nodeToRegister;
            string clientConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;

            // authorization token
            string token = headers["Authorization"];

            string nodeIdFrom = headers["x-andyx-node-id-from"].ToString();
            var replicaClient = _clusterRepository.GetReplica(nodeIdFrom);
            if (replicaClient == null)
                throw new Exception($"Invalid node {nodeIdFrom}, this node is not registered in the cluster configuration of this node., cluster is shutting down.");

            string clusterId = headers["x-andyx-cluster-id"].ToString();
            string nodeId = replicaClient.NodeId;
            string hostName = replicaClient.Host;
            int shardId = Convert.ToInt32(headers["x-andyx-shard-id"].ToString());

            ReplicaTypes replicaType = replicaClient.Type;
            _logger.LogInformation($"Node '{nodeId}' as {replicaType.ToString()} with hostname '{hostName}' requested connection");


            // check if this node is already connected here
            if (_clusterHubRepository.GetNodeClientByNodeId(nodeId) != null)
            {
                // node exists
                _logger.LogWarning($"Node '{nodeId}' as {replicaType.ToString()} with hostname '{hostName}' is already connected  ");
                return OnDisconnectedAsync(new Exception($"There is a node with id '{nodeId}' part of cluster '{clusterId}' connected to this node"));
            }

            nodeToRegister = _clusterFactory.CreateNodeClient(nodeId, hostName, shardId, replicaType);
            _clusterHubRepository.AddNodeClient(clientConnectionId, nodeToRegister);

            Clients.Caller.NodeConnectedAsync(new Model.Clusters.Events.NodeConnectedArgs()
            {
                HostName = hostName,
                NodeId = nodeId,
                ReplicaType = replicaType.ToString(),
                ShardId = shardId
            });

            _logger.LogInformation($"Node '{nodeId}' as {replicaType.ToString()} with hostname '{hostName}' is connected");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientConnectionId = Context.ConnectionId;
            var nodeClientToRemove = _clusterHubRepository.GetNodeClientByNodeId(clientConnectionId);
            if (nodeClientToRemove != null)
            {
                _clusterHubRepository.RemoveNodeClient(clientConnectionId);

                _logger.LogInformation($"Node '{nodeClientToRemove.NodeId}' with hostname '{nodeClientToRemove.HostName}' is disconnected");

                Clients.Caller.NodeDisconnectedAsync(new Model.Clusters.Events.NodeDisconnectedArgs()
                {
                    HostName = nodeClientToRemove.HostName,
                    NodeId = nodeClientToRemove.NodeId,
                    ReplicaType = nodeClientToRemove.ReplicaType.ToString(),
                    ShardId = nodeClientToRemove.ShardId
                });
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
