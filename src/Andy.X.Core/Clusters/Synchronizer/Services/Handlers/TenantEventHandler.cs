using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class TenantEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        public TenantEventHandler(NodeClusterEventService nodeClusterEventService)
        {
            _nodeClusterEventService = nodeClusterEventService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.TenantCreated += NodeClusterEventService_TenantCreated;
            _nodeClusterEventService.TenantUpdated += NodeClusterEventService_TenantUpdated;
            _nodeClusterEventService.TenantDeleted += NodeClusterEventService_TenantDeleted;
        }

        private void NodeClusterEventService_TenantUpdated(Buildersoft.Andy.X.Model.Clusters.Events.TenantUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_TenantDeleted(Buildersoft.Andy.X.Model.Clusters.Events.TenantDeletedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_TenantCreated(Buildersoft.Andy.X.Model.Clusters.Events.TenantCreatedArgs obj)
        {
            throw new NotImplementedException();
        }
    }
}
