using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class TopicEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantStateService _tenantService;
        private readonly ITenantFactory _tenantFactory;
        private readonly ICoreService _coreService;

        public TopicEventHandler(NodeClusterEventService nodeClusterEventService,
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
            _nodeClusterEventService.TopicCreated += NodeClusterEventService_TopicCreated;
            _nodeClusterEventService.TopicUpdated += NodeClusterEventService_TopicUpdated;
            _nodeClusterEventService.TopicDeleted += NodeClusterEventService_TopicDeleted;
        }

        private void NodeClusterEventService_TopicUpdated(Model.Clusters.Events.TopicUpdatedArgs obj)
        {
            try
            {
                _coreService.UpdateTopicSettings(obj.Tenant, obj.Product, obj.Component, obj.Name, obj.TopicSettings, false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_TopicDeleted(Model.Clusters.Events.TopicDeletedArgs obj)
        {
            try
            {
                _coreService.DeleteTopic(obj.Tenant, obj.Product, obj.Component, obj.Name, false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_TopicCreated(Model.Clusters.Events.TopicCreatedArgs obj)
        {
            try
            {
                var topicToAdd = _tenantFactory.CreateTopic(obj.Topic.Name, obj.Topic.Description);
                _coreService.CreateTopic(obj.Tenant, obj.Product, obj.Component, obj.Topic.Name, obj.Topic.Description, obj.TopicSettings, false);
                _tenantService.AddTopic(obj.Tenant, obj.Product, obj.Component, obj.Topic.Name, topicToAdd, storeProductIntoCore: false);
            }
            catch (Exception)
            {

            }
        }
    }
}
