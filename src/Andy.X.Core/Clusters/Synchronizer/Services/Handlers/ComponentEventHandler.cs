using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class ComponentEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantStateService _tenantService;
        private readonly ITenantFactory _tenantFactory;
        private readonly ICoreService _coreService;

        public ComponentEventHandler(NodeClusterEventService nodeClusterEventService,
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
            try
            {
                _coreService.DeleteComponentToken(obj.Tenant, obj.Product, obj.Component, obj.Key, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ComponentTokenRevoked(Model.Clusters.Events.ComponentTokenRevokedArgs obj)
        {
            try
            {
                _coreService.RevokeComponentToken(obj.Tenant, obj.Product, obj.Component, obj.Key, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ComponentTokenCreated(Model.Clusters.Events.ComponentTokenCreatedArgs obj)
        {
            try
            {
                _coreService.CreateComponentToken(obj.Tenant, obj.Product, obj.Component, obj.ComponentToken, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_ComponentRetentionUpdated(Model.Clusters.Events.ComponentRetentionUpdatedArgs obj)
        {
            try
            {
                _coreService.UpdateComponentRetention(obj.Tenant,
                    obj.Product,
                    obj.Component,
                    obj.ComponentRetention.Type,
                    obj.ComponentRetention.Name,
                    obj.ComponentRetention.TimeToLiveInMinutes, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ComponentRetentionDeleted(Model.Clusters.Events.ComponentRetentionDeletedArgs obj)
        {
            try
            {
                _coreService.DeleteComponentRetention(obj.Tenant, obj.Product, obj.Component, obj.ComponentRetention.Type, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ComponentRetentionCreated(Model.Clusters.Events.ComponentRetentionCreatedArgs obj)
        {
            try
            {
                _coreService.CreateComponentRetention(obj.Tenant,
                    obj.Product,
                    obj.Component,
                    obj.ComponentRetention.Name,
                    obj.ComponentRetention.Type,
                    obj.ComponentRetention.TimeToLiveInMinutes, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_ComponentUpdated(Model.Clusters.Events.ComponentUpdatedArgs obj)
        {
            try
            {
                _coreService.UpdateComponentSettings(obj.Tenant,
                    obj.Product,
                    obj.Component,
                    obj.Settings.IsTopicAutomaticCreationAllowed,
                    obj.Settings.EnforceSchemaValidation,
                    obj.Settings.IsAuthorizationEnabled,
                    obj.Settings.IsSubscriptionAutomaticCreationAllowed,
                    obj.Settings.IsProducerAutomaticCreationAllowed, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ComponentDeleted(Model.Clusters.Events.ComponentDeletedArgs obj)
        {
            try
            {
                _coreService.DeleteComponent(obj.Tenant, obj.Product, obj.Name, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ComponentCreated(Model.Clusters.Events.ComponentCreatedArgs obj)
        {
            try
            {
                var componentToAdd = _tenantFactory.CreateComponent(obj.Name, obj.Component.Description, obj.Settings);
                _tenantService.AddComponent(obj.Tenant, obj.Product, obj.Name, componentToAdd, storeProductIntoCore: false);

                _coreService.CreateComponent(
                    obj.Tenant,
                    obj.Product,
                    obj.Name,
                    obj.Component.Description,
                    obj.Settings.IsTopicAutomaticCreationAllowed,
                    obj.Settings.EnforceSchemaValidation,
                    obj.Settings.IsAuthorizationEnabled,
                    obj.Settings.IsSubscriptionAutomaticCreationAllowed,
                    obj.Settings.IsProducerAutomaticCreationAllowed, notifyCluster: false);

            }
            catch (Exception)
            {

            }
        }
    }
}
