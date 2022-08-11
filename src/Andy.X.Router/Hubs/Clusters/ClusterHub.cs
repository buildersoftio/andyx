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

        public ClusterHub(ILogger<ClusterHub> logger,
            IClusterService clusterService,
            IClusterHubRepository clusterHubRepository,
            IClusterFactory clusterFactory)
        {
            _logger = logger;

            _clusterService = clusterService;
            _clusterHubRepository = clusterHubRepository;
            _clusterFactory = clusterFactory;
        }

        public override Task OnConnectedAsync()
        {
            NodeClient nodeToRegister;
            string clientConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;

            // authorization token
            string token = headers["Authorization"];

            string clusterId = headers["x-andyx-cluster-id"].ToString();
            string nodeId = headers["x-andyx-node-id"].ToString();
            string nodeIdFrom = headers["x-andyx-node-id-from"].ToString();
            string hostName = headers["x-andyx-hostname"].ToString();
            int shardId = Convert.ToInt32(headers["x-andyx-shard-id"].ToString());

            ReplicaTypes replicaType = (ReplicaTypes)Enum.Parse(typeof(ReplicaTypes), headers["x-andyx-replica-type"].ToString());
            _logger.LogInformation($"Node '{nodeId}' as {replicaType.ToString()} with hostname '{hostName}' requested connection from '{nodeIdFrom}'");


            // check if this node is already connected here
            if (_clusterHubRepository.GetNodeClientByNodeId(nodeId) != null)
            {
                // node exists
                _logger.LogWarning($"Node '{nodeId}' as {replicaType.ToString()} with hostname '{hostName}' is already connected  from '{nodeIdFrom}'");
                return OnDisconnectedAsync(new Exception($"There is a node with id '{nodeId}' part of cluster '{clusterId}' connected to this node  from '{nodeIdFrom}'"));
            }

            nodeToRegister = _clusterFactory.CreateNodeClient(nodeId, hostName, shardId, replicaType);
            _clusterHubRepository.AddNodeClient(clientConnectionId, nodeToRegister);

            // BUG: Here we have registered only this node properties (should be clients from)
            Clients.Caller.NodeConnectedAsync(new Model.Clusters.Events.NodeConnectedArgs()
            {
                HostName = hostName,
                NodeId = nodeId,
                ReplicaType = replicaType.ToString(),
                ShardId = shardId
            });
            _logger.LogInformation($"Node '{nodeId}' as {replicaType.ToString()} with hostname '{hostName}' is connected  from '{nodeIdFrom}'");

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
