namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class NodeEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        public NodeEventHandler(NodeClusterEventService nodeClusterEventService)
        {
            _nodeClusterEventService = nodeClusterEventService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.NodeConnected += NodeClusterEventService_NodeConnected;
            _nodeClusterEventService.NodeDisconnected += NodeClusterEventService_NodeDisconnected;
        }

        private void NodeClusterEventService_NodeDisconnected(Model.Clusters.Events.NodeDisconnectedArgs obj)
        {
            // TODO: Implement
        }

        private void NodeClusterEventService_NodeConnected(Model.Clusters.Events.NodeConnectedArgs obj)
        {
            // TODO: Implement
        }
    }
}
