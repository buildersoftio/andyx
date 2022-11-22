using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class ProductEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ITenantStateService _tenantService;
        private readonly ITenantFactory _tenantFactory;
        private readonly ICoreService _coreService;

        public ProductEventHandler(NodeClusterEventService nodeClusterEventService,
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
            try
            {
                // do not notify the cluster as this call is happening with request of other nodes.
                _coreService.DeleteProductToken(obj.Tenant, obj.Product, obj.Key, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ProductTokenRevoked(Model.Clusters.Events.ProductTokenRevokedArgs obj)
        {
            try
            {
                // do not notify the cluster as this call is happening with request of other nodes.
                _coreService.RevokeProductToken(obj.Tenant, obj.Product, obj.Key, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ProductTokenCreated(Model.Clusters.Events.ProductTokenCreatedArgs obj)
        {
            try
            {
                // do not notify the cluster as this call is happening with request of other nodes.
                _coreService.CreateProductToken(obj.Tenant, obj.Product, obj.ProductToken, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_ProductRetentionUpdated(Model.Clusters.Events.ProductRetentionUpdatedArgs obj)
        {
            try
            {
                _coreService.UpdateProductRetention(obj.Tenant,
                    obj.Product,
                    obj.ProductRetention.Type,
                    obj.ProductRetention.Name,
                    obj.ProductRetention.TimeToLiveInMinutes, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ProductRetentionDeleted(Model.Clusters.Events.ProductRetentionDeletedArgs obj)
        {
            try
            {
                _coreService.DeleteProductRetention(obj.Tenant, obj.Product, obj.ProductRetention.Type, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ProductRetentionCreated(Model.Clusters.Events.ProductRetentionCreatedArgs obj)
        {
            try
            {
                _coreService.CreateProductRetention(obj.Tenant,
                    obj.Product,
                    obj.ProductRetention.Name,
                    obj.ProductRetention.Type,
                    obj.ProductRetention.TimeToLiveInMinutes, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_ProductUpdated(Model.Clusters.Events.ProductUpdatedArgs obj)
        {
            try
            {
                _coreService.UpdateProductSettings(obj.Tenant, obj.Name, obj.ProductSettings.IsAuthorizationEnabled, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ProductDeleted(Model.Clusters.Events.ProductDeletedArgs obj)
        {
            try
            {
                _coreService.DeleteProduct(obj.Tenant, obj.Name, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
        private void NodeClusterEventService_ProductCreated(Model.Clusters.Events.ProductCreatedArgs obj)
        {
            try
            {
                var productToRegister = _tenantFactory.CreateProduct(obj.Name, obj.Description);
                _tenantService.AddProduct(obj.Tenant, obj.Name, productToRegister, false);
                _coreService.CreateProduct(obj.Tenant, obj.Name, obj.Description, obj.ProductSettings.IsAuthorizationEnabled, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }
    }
}
