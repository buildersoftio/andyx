using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class ProductEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        public ProductEventHandler(NodeClusterEventService nodeClusterEventService)
        {
            _nodeClusterEventService = nodeClusterEventService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.ProductCreated += NodeClusterEventService_ProductCreated;
            _nodeClusterEventService.ProductUpdated += NodeClusterEventService_ProductUpdated;
            _nodeClusterEventService.ProductDeleted += NodeClusterEventService_ProductDeleted;
        }

        private void NodeClusterEventService_ProductUpdated(Model.Clusters.Events.ProductUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ProductDeleted(Model.Clusters.Events.ProductDeletedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ProductCreated(Model.Clusters.Events.ProductCreatedArgs obj)
        {
            throw new NotImplementedException();
        }
    }
}
