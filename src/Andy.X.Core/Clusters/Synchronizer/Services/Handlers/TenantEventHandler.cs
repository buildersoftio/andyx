
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class TenantEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantService _tenantService;
        private readonly ITenantFactory _tenantFactory;

        public TenantEventHandler(NodeClusterEventService nodeClusterEventService, ITenantService tenantService, ITenantFactory tenantFactory)
        {
            _nodeClusterEventService = nodeClusterEventService;
            _tenantService = tenantService;
            _tenantFactory = tenantFactory;
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.TenantCreated += NodeClusterEventService_TenantCreated;
            _nodeClusterEventService.TenantUpdated += NodeClusterEventService_TenantUpdated;
            _nodeClusterEventService.TenantDeleted += NodeClusterEventService_TenantDeleted;
        }

        private void NodeClusterEventService_TenantUpdated(Model.Clusters.Events.TenantUpdatedArgs obj)
        {
            // TODO: Not implemented.
        }

        private void NodeClusterEventService_TenantCreated(Model.Clusters.Events.TenantCreatedArgs obj)
        {
            try
            {
                var tenantToCreate = _tenantFactory.CreateTenant(obj.Id, obj.Name, obj.Settings);
                _tenantService.AddTenant(obj.Name, tenantToCreate, notifyOtherNodes: false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_TenantDeleted(Model.Clusters.Events.TenantDeletedArgs obj)
        {
            // TODO: Not implemented.
        }
    }
}
