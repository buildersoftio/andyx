namespace Andy.X.Cluster.Sync.Services.Handlers
{
    public class ProducerEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        public ProducerEventHandler(NodeClusterEventService nodeClusterEventService)
        {
            _nodeClusterEventService = nodeClusterEventService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.ProducerConnected += NodeClusterEventService_ProducerConnected;
            _nodeClusterEventService.ProducerDisconnected += NodeClusterEventService_ProducerDisconnected;
        }

        private void NodeClusterEventService_ProducerDisconnected(Buildersoft.Andy.X.Model.Producers.Events.ProducerDisconnectedDetails obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ProducerConnected(Buildersoft.Andy.X.Model.Producers.Events.ProducerConnectedDetails obj)
        {
            throw new NotImplementedException();
        }
    }
}
