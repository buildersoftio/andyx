using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class ComponentEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantStateService _tenantService;
        private readonly ITenantFactory _tenantFactory;

        public ComponentEventHandler(NodeClusterEventService nodeClusterEventService, ITenantStateService tenantService, ITenantFactory tenantFactory)
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

            _nodeClusterEventService.ComponentRetentionCreated += NodeClusterEventService_ComponentRetentionCreated;
            _nodeClusterEventService.ComponentRetentionDeleted += NodeClusterEventService_ComponentRetentionDeleted;
            _nodeClusterEventService.ComponentRetentionUpdated += NodeClusterEventService_ComponentRetentionUpdated;

            _nodeClusterEventService.ComponentTokenCreated += NodeClusterEventService_ComponentTokenCreated;
            _nodeClusterEventService.ComponentTokenRevoked += NodeClusterEventService_ComponentTokenRevoked;
            _nodeClusterEventService.ComponentTokenDeleted += NodeClusterEventService_ComponentTokenDeleted;
        }

        private void NodeClusterEventService_ComponentTokenDeleted(Model.Clusters.Events.ComponentTokenDeletedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ComponentTokenRevoked(Model.Clusters.Events.ComponentTokenRevokedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ComponentTokenCreated(Model.Clusters.Events.ComponentTokenCreatedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ComponentRetentionUpdated(Model.Clusters.Events.ComponentRetentionUpdatedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ComponentRetentionDeleted(Model.Clusters.Events.ComponentRetentionDeletedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ComponentRetentionCreated(Model.Clusters.Events.ComponentRetentionCreatedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ComponentUpdated(Model.Clusters.Events.ComponentUpdatedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ComponentDeleted(Model.Clusters.Events.ComponentDeletedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ComponentCreated(Model.Clusters.Events.ComponentCreatedArgs obj)
        {
            var componentToAdd = _tenantFactory.CreateComponent(obj.Name, obj.Description, obj.Settings);
            _tenantService.AddComponent(obj.Tenant, obj.Product, obj.Name, componentToAdd, notifyOtherNodes: false);
        }
    }
}
