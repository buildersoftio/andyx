using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class ProductEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantService _tenantService;
        private readonly ITenantFactory _tenantFactory;

        public ProductEventHandler(NodeClusterEventService nodeClusterEventService,
            ITenantService tenantService,
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
        }

        private void NodeClusterEventService_ProductUpdated(Model.Clusters.Events.ProductUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ProductDeleted(Model.Clusters.Events.ProductDeletedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_ProductCreated(Model.Clusters.Events.ProductCreatedArgs obj)
        {
            var productToRegister = _tenantFactory.CreateProduct(obj.Name, obj.Description, obj.ProductOwner, obj.ProductTeam, obj.ProductContact);
            _tenantService.AddProduct(obj.Tenant, obj.Name, productToRegister, notifyOtherNodes: false);
        }
    }
}
