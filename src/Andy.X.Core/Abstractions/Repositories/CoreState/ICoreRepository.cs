﻿using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Subscriptions;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Buildersoft.Andy.X.Core.Contexts.CoreState;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState
{
    public interface ICoreRepository
    {
        List<Tenant> GetTenants(bool withInActive = true);
        void AddTenant(Tenant tenant);
        void EditTenant(Tenant tenant);
        void SoftDeleteTenant(long tenantId);
        void DeleteTenant(long tenantId);
        Tenant GetTenant(long tenantId);
        Tenant GetTenant(string tenantName);

        void AddTenantSettings(TenantSettings tenantSettings);
        void EditTenantSettings(TenantSettings tenantSettings);
        void DeleteTenantSettings(long tenantId);
        TenantSettings GetTenantSettings(long tenantId);

        void AddTenantToken(TenantToken tenantToken);
        void EditTenantToken(TenantToken tenantToken);
        void DeleteTenantToken(Guid id);
        TenantToken GetTenantToken(Guid id);
        List<TenantToken> GetTenantToken(long tenantId);

        void AddTenantRetention(TenantRetention tenantRetention);
        void EditTenantRetention(TenantRetention tenantRetention);
        void DeleteTenantRetention(long retentionId);
        TenantRetention GetTenantRetention(long retentionId);
        List<TenantRetention> GetTenantRetentions(long tenantId);

        List<Product> GetProducts(long tenantId);
        void AddProduct(Product product);
        void EditProduct(Product product);
        void DeleteProduct(long productId);
        void SoftDeleteProduct(long productId);
        Product GetProduct(long productId);
        Product GetProduct(long tenantId, string productName);

        void AddProductToken(ProductToken productToken);
        void EditProductToken(ProductToken productToken);
        void DeleteProductToken(Guid id);
        ProductToken GetProductToken(Guid id);
        List<ProductToken> GetProductToken(long productId);


        void AddProductSettings(ProductSettings productSettings);
        void EditProductSettings(ProductSettings productSettings);
        void DeleteProductSettings(long productId);
        ProductSettings GetProductSettings(long productId);

        void AddProductRetention(ProductRetention productRetention);
        void EditProductRetention(ProductRetention productRetention);
        void DeleteProductRetention(long retentionId);
        ProductRetention GetProductRetention(long retentionId);
        List<ProductRetention> GetProductRetentions(long productId);

        List<Component> GetComponents(long productId);
        void AddComponent(Component component);
        void EditComponent(Component component);
        void DeleteComponent(long componentId);
        void SoftDeleteComponent(long componentId);
        Component GetComponent(long componentId);
        Component GetComponent(long tenantId, long productId, string componentName);

        void AddComponentToken(ComponentToken componentToken);
        void EditComponentToken(ComponentToken componentToken);
        void DeleteComponentToken(Guid id);
        ComponentToken GetComponentToken(Guid id);
        List<ComponentToken> GetComponentToken(long componentId);

        void AddComponentSettings(ComponentSettings componentSettings);
        void EditComponentSettings(ComponentSettings componentSettings);
        void DeleteComponentSettings(long componentId);
        ComponentSettings GetComponentSettings(long componentId);

        void AddComponentRetention(ComponentRetention componentRetention);
        void EditComponentRetention(ComponentRetention componentRetention);
        void DeleteComponentRetention(long retentionId);
        ComponentRetention GetComponentRetention(long retentionId);
        List<ComponentRetention> GetComponentRetentions(long componentId);

        List<Topic> GetTopics(long componentId);
        void AddTopic(Topic topic);
        void EditTopic(Topic topic);
        void DeleteTopic(long topicId);
        void SoftDeleteTopic(long topicId);
        Topic GetTopic(long topicId);
        Topic GetTopic(long componentId, string topic);

        List<Subscription> GetSubscriptions(long topicId);  
        void AddSubscription(Subscription subscription);
        void EditSubscription(Subscription subscription);
        void DeleteSubscription(long subscriptionId);
        void SoftDeleteSubscription(long subscriptionId);
        Subscription GetSubscription(long subscriptionId);
        Subscription GetSubscription(long topicId, string subscriptionName);


        int SaveChanges();
        Task<int> SaveChangesAsync();

        CoreStateContext GetCoreStateContext();
    }
}
