using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class SubscriptionEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        public SubscriptionEventHandler(NodeClusterEventService nodeClusterEventService)
        {
            _nodeClusterEventService = nodeClusterEventService;

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

        private void NodeClusterEventService_CurrentEntryPositionUpdated(Buildersoft.Andy.X.Model.Clusters.Events.CurrentEntryPositionUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_SubscriptionPositionUpdated(Buildersoft.Andy.X.Model.Clusters.Events.SubscriptionPositionUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_SubscriptionUpdated(Buildersoft.Andy.X.Model.Clusters.Events.SubscriptionUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_SubscriptionDeleted(Buildersoft.Andy.X.Model.Clusters.Events.SubscriptionDeletedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_SubscriptionCreated(Buildersoft.Andy.X.Model.Clusters.Events.SubscriptionCreatedArgs obj)
        {
            throw new NotImplementedException();
        }
    }
}
