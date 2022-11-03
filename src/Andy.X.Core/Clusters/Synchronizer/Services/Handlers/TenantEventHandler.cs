
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class TenantEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantStateService _tenantService;
        private readonly ITenantFactory _tenantFactory;
        private readonly ICoreService _coreService;

        public TenantEventHandler(NodeClusterEventService nodeClusterEventService,
            ITenantStateService tenantService,
            ITenantFactory tenantFactory,
            ICoreService coreService)
        {
            _nodeClusterEventService = nodeClusterEventService;
            _tenantService = tenantService;
            _tenantFactory = tenantFactory;
            _coreService = coreService;

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
            try
            {
                // do not notify the cluster as this call is happening with request of other nodes.
                _coreService.RevokeTenantToken(obj.Tenant, obj.Key, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_TenantTokenDeleted(Model.Clusters.Events.TenantTokenDeletedArgs obj)
        {
            try
            {
                // do not notify the cluster as this call is happening with request of other nodes.
                _coreService.DeleteTenantToken(obj.Tenant, obj.Key, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_TenantTokenCreated(Model.Clusters.Events.TenantTokenCreatedArgs obj)
        {
            try
            {
                // do not notify the cluster as this call is happening with request of other nodes.
                _coreService.CreateTenantToken(obj.Tenant, obj.TenantToken, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_TenantRetentionUpdated(Model.Clusters.Events.TenantRetentionUpdatedArgs obj)
        {
            try
            {
                _coreService.UpdateTenantRetention(obj.Tenant,
                    obj.TenantRetention.Type,
                    obj.TenantRetention.Name,
                    obj.TenantRetention.TimeToLiveInMinutes, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_TenantRetentionCreated(Model.Clusters.Events.TenantRetentionCreatedArgs obj)
        {
            try
            {
                _coreService.CreateTenantRetention(obj.Tenant,
                    obj.TenantRetention.Name,
                    obj.TenantRetention.Type,
                    obj.TenantRetention.TimeToLiveInMinutes, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_TenantRetentionDeleted(Model.Clusters.Events.TenantRetentionDeletedArgs obj)
        {
            try
            {
                _coreService.DeleteTenantRetention(obj.Tenant, obj.TenantRetention.Type, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_TenantUpdated(Model.Clusters.Events.TenantUpdatedArgs obj)
        {
            try
            {
                _coreService.UpdateTenantSettings(obj.Name,
                    obj.Settings.IsProductAutomaticCreationAllowed,
                    obj.Settings.IsEncryptionEnabled,
                    obj.Settings.IsAuthorizationEnabled,
                    notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_TenantCreated(Model.Clusters.Events.TenantCreatedArgs obj)
        {
            try
            {
                var tenantToCreate = _tenantFactory.CreateTenant(obj.Name, obj.Settings);
                _tenantService.AddTenant(obj.Name, tenantToCreate, notifyOtherNodes: false);
                _coreService.CreateTenant(obj.Name, obj.Settings.IsProductAutomaticCreationAllowed, obj.Settings.IsEncryptionEnabled, obj.Settings.IsAuthorizationEnabled);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_TenantDeleted(Model.Clusters.Events.TenantDeletedArgs obj)
        {
            try
            {
                _coreService.DeleteTenant(obj.Name, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
    }
}
