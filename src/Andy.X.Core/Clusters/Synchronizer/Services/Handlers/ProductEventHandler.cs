using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class ProductEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantStateService _tenantService;
        private readonly ITenantFactory _tenantFactory;

        public ProductEventHandler(NodeClusterEventService nodeClusterEventService,
            ITenantStateService tenantService,
            ITenantFactory tenantFactory)
        {
            _nodeClusterEventService = nodeClusterEventService;

            _tenantService = tenantService;
            _tenantFactory = tenantFactory;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.ProductCreated += NodeClusterEventService_ProductCreated;
            _nodeClusterEventService.ProductUpdated += NodeClusterEventService_ProductUpdated;
            _nodeClusterEventService.ProductDeleted += NodeClusterEventService_ProductDeleted;

            _nodeClusterEventService.ProductRetentionCreated += NodeClusterEventService_ProductRetentionCreated;
            _nodeClusterEventService.ProductRetentionDeleted += NodeClusterEventService_ProductRetentionDeleted;
            _nodeClusterEventService.ProductRetentionUpdated += NodeClusterEventService_ProductRetentionUpdated;

            _nodeClusterEventService.ProductTokenCreated += NodeClusterEventService_ProductTokenCreated;
            _nodeClusterEventService.ProductTokenRevoked += NodeClusterEventService_ProductTokenRevoked;
            _nodeClusterEventService.ProductTokenDeleted += NodeClusterEventService_ProductTokenDeleted;
        }

        private void NodeClusterEventService_ProductTokenDeleted(Model.Clusters.Events.ProductTokenDeletedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ProductTokenRevoked(Model.Clusters.Events.ProductTokenRevokedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ProductTokenCreated(Model.Clusters.Events.ProductTokenCreatedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ProductRetentionUpdated(Model.Clusters.Events.ProductRetentionUpdatedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ProductRetentionDeleted(Model.Clusters.Events.ProductRetentionDeletedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ProductRetentionCreated(Model.Clusters.Events.ProductRetentionCreatedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ProductUpdated(Model.Clusters.Events.ProductUpdatedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ProductDeleted(Model.Clusters.Events.ProductDeletedArgs obj)
        {
            
        }

        private void NodeClusterEventService_ProductCreated(Model.Clusters.Events.ProductCreatedArgs obj)
        {
            var productToRegister = _tenantFactory.CreateProduct(obj.Name, obj.Description);
            _tenantService.AddProduct(obj.Tenant, obj.Name, productToRegister, notifyOtherNodes: false);
        }
    }
}
