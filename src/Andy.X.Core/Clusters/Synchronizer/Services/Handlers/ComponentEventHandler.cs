using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class ComponentEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantService _tenantService;
        private readonly ITenantFactory _tenantFactory;

        public ComponentEventHandler(NodeClusterEventService nodeClusterEventService, ITenantService tenantService, ITenantFactory tenantFactory)
        {
            _nodeClusterEventService = nodeClusterEventService;
            this._tenantService = tenantService;
            _tenantFactory = tenantFactory;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.ComponentCreated += NodeClusterEventService_ComponentCreated;
            _nodeClusterEventService.ComponentUpdated += NodeClusterEventService_ComponentUpdated;
            _nodeClusterEventService.ComponentDeleted += NodeClusterEventService_ComponentDeleted;
        }

        private void NodeClusterEventService_ComponentUpdated(Model.Clusters.Events.ComponentUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ComponentDeleted(Model.Clusters.Events.ComponentDeletedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ComponentCreated(Model.Clusters.Events.ComponentCreatedArgs obj)
        {
            var componentToAdd = _tenantFactory.CreateComponent(obj.Id, obj.Name, obj.Settings);
            _tenantService.AddComponent(obj.Tenant, obj.Product, obj.Name, componentToAdd, notifyOtherNodes: false);
        }
    }
}
