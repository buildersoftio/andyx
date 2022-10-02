
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class TenantEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantStateService _tenantService;
        private readonly ITenantFactory _tenantFactory;

        public TenantEventHandler(NodeClusterEventService nodeClusterEventService, ITenantStateService tenantService, ITenantFactory tenantFactory)
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

            _nodeClusterEventService.TenantRetentionCreated += NodeClusterEventService_TenantRetentionCreated;
            _nodeClusterEventService.TenantRetentionUpdated += NodeClusterEventService_TenantRetentionUpdated;
            _nodeClusterEventService.TenantRetentionDeleted += NodeClusterEventService_TenantRetentionDeleted;

            _nodeClusterEventService.TenantTokenCreated += NodeClusterEventService_TenantTokenCreated;
            _nodeClusterEventService.TenantTokenDeleted += NodeClusterEventService_TenantTokenDeleted;
            _nodeClusterEventService.TenantTokenRevoked += NodeClusterEventService_TenantTokenRevoked;
        }

        private void NodeClusterEventService_TenantTokenRevoked(Model.Clusters.Events.TenantTokenRevokedArgs obj)
        {

        }

        private void NodeClusterEventService_TenantTokenDeleted(Model.Clusters.Events.TenantTokenDeletedArgs obj)
        {

        }

        private void NodeClusterEventService_TenantTokenCreated(Model.Clusters.Events.TenantTokenCreatedArgs obj)
        {

        }

        private void NodeClusterEventService_TenantRetentionUpdated(Model.Clusters.Events.TenantRetentionUpdatedArgs obj)
        {

        }

        private void NodeClusterEventService_TenantRetentionCreated(Model.Clusters.Events.TenantRetentionCreatedArgs obj)
        {

        }

        private void NodeClusterEventService_TenantRetentionDeleted(Model.Clusters.Events.TenantRetentionDeletedArgs obj)
        {

        }

        private void NodeClusterEventService_TenantUpdated(Model.Clusters.Events.TenantUpdatedArgs obj)
        {
            // TODO: Not implemented.
        }

        private void NodeClusterEventService_TenantCreated(Model.Clusters.Events.TenantCreatedArgs obj)
        {
            try
            {
                var tenantToCreate = _tenantFactory.CreateTenant(obj.Name, obj.Settings);
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
