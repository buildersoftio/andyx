using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class TopicEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantStateService _tenantService;
        private readonly ITenantFactory _tenantFactory;

        public TopicEventHandler(NodeClusterEventService nodeClusterEventService, ITenantStateService tenantService, ITenantFactory tenantFactory)
        {
            _nodeClusterEventService = nodeClusterEventService;

            _tenantService = tenantService;
            _tenantFactory = tenantFactory;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.TopicCreated += NodeClusterEventService_TopicCreated;
            _nodeClusterEventService.TopicUpdated += NodeClusterEventService_TopicUpdated;
            _nodeClusterEventService.TopicDeleted += NodeClusterEventService_TopicDeleted;
        }

        private void NodeClusterEventService_TopicUpdated(Model.Clusters.Events.TopicUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_TopicDeleted(Model.Clusters.Events.TopicDeletedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_TopicCreated(Model.Clusters.Events.TopicCreatedArgs obj)
        {
            var topicToAdd = _tenantFactory.CreateTopic(obj.Name, obj.Description);
            _tenantService.AddTopic(obj.Tenant, obj.Product, obj.Component, obj.Name, topicToAdd, notifyOtherNodes: false);
        }
    }
}
