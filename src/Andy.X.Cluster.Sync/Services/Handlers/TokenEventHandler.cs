namespace Andy.X.Cluster.Sync.Services.Handlers
{
    public class TokenEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        public TokenEventHandler(NodeClusterEventService nodeClusterEventService)
        {
            _nodeClusterEventService = nodeClusterEventService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.TokenCreated += NodeClusterEventService_TokenCreated;
            _nodeClusterEventService.TokenRevoked += NodeClusterEventService_TokenRevoked;
        }

        private void NodeClusterEventService_TokenRevoked(Buildersoft.Andy.X.Model.Clusters.Events.TokenRevokedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_TokenCreated(Buildersoft.Andy.X.Model.Clusters.Events.TokenCreatedArgs obj)
        {
            throw new NotImplementedException();
        }
    }
}
