namespace Andy.X.Cluster.Sync.Services.Handlers
{
    public class ConsumerEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        public ConsumerEventHandler(NodeClusterEventService nodeClusterEventService)
        {
            _nodeClusterEventService = nodeClusterEventService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.ConsumerConnected += NodeClusterEventService_ConsumerConnected;
            _nodeClusterEventService.ConsumerDisconnected += NodeClusterEventService_ConsumerDisconnected;
        }

        private void NodeClusterEventService_ConsumerDisconnected(Buildersoft.Andy.X.Model.Consumers.Events.ConsumerDisconnectedDetails obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ConsumerConnected(Buildersoft.Andy.X.Model.Consumers.Events.ConsumerConnectedDetails obj)
        {
            throw new NotImplementedException();
        }
    }
}
