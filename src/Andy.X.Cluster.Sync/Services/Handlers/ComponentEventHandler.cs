namespace Andy.X.Cluster.Sync.Services.Handlers
{
    public class ComponentEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        public ComponentEventHandler(NodeClusterEventService nodeClusterEventService)
        {
            _nodeClusterEventService = nodeClusterEventService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.ComponentCreated += NodeClusterEventService_ComponentCreated;
            _nodeClusterEventService.ComponentUpdated += NodeClusterEventService_ComponentUpdated;
            _nodeClusterEventService.ComponentDeleted += NodeClusterEventService_ComponentDeleted;
        }

        private void NodeClusterEventService_ComponentUpdated(Buildersoft.Andy.X.Model.Clusters.Events.ComponentUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ComponentDeleted(Buildersoft.Andy.X.Model.Clusters.Events.ComponentDeletedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ComponentCreated(Buildersoft.Andy.X.Model.Clusters.Events.ComponentCreatedArgs obj)
        {
            throw new NotImplementedException();
        }
    }
}
