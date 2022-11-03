using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class SubscriptionEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantStateService _tenantService;
        private readonly ISubscriptionFactory _subscriptionFactory;
        private readonly ICoreService _coreService;

        public SubscriptionEventHandler(NodeClusterEventService nodeClusterEventService,
            ITenantStateService tenantService,
            ISubscriptionFactory subscriptionFactory,
            ICoreService coreService)
        {
            _nodeClusterEventService = nodeClusterEventService;
            _tenantService = tenantService;
            _subscriptionFactory = subscriptionFactory;
            _coreService = coreService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.SubscriptionCreated += NodeClusterEventService_SubscriptionCreated;
            _nodeClusterEventService.SubscriptionUpdated += NodeClusterEventService_SubscriptionUpdated;
            _nodeClusterEventService.SubscriptionDeleted += NodeClusterEventService_SubscriptionDeleted;

            _nodeClusterEventService.SubscriptionPositionUpdated += NodeClusterEventService_SubscriptionPositionUpdated;
            _nodeClusterEventService.CurrentEntryPositionUpdated += NodeClusterEventService_CurrentEntryPositionUpdated;
        }

        private void NodeClusterEventService_CurrentEntryPositionUpdated(Model.Clusters.Events.CurrentEntryPositionUpdatedArgs obj)
        {
            // TODO: Not implemented.
        }

        private void NodeClusterEventService_SubscriptionPositionUpdated(Model.Clusters.Events.SubscriptionPositionUpdatedArgs obj)
        {
            // TODO: Not implemented.
        }

        private void NodeClusterEventService_SubscriptionUpdated(Model.Clusters.Events.SubscriptionUpdatedArgs obj)
        {
            // TODO: Not implemented.
        }

        private void NodeClusterEventService_SubscriptionDeleted(Model.Clusters.Events.SubscriptionDeletedArgs obj)
        {
            try
            {
                _coreService.DeleteSubscription(obj.Tenant, obj.Product, obj.Component, obj.Topic, obj.SubscriptionName, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_SubscriptionCreated(Model.Clusters.Events.SubscriptionCreatedArgs obj)
        {
            try
            {
                var subscriptionToAdd = _subscriptionFactory.CreateSubscription(obj.Tenant, obj.Product, obj.Component, obj.Topic, obj.SubscriptionName,
                    obj.SubscriptionType, obj.SubscriptionMode, obj.InitialPosition);

                _tenantService.AddSubscriptionConfiguration(obj.Tenant, obj.Product, obj.Component, obj.Topic, obj.SubscriptionName, subscriptionToAdd, notifyOtherNodes: false);
            }
            catch (Exception)
            {

            }
        }
    }
}
