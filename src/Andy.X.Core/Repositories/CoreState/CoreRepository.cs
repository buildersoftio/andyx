using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Contexts.CoreState;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Subscriptions;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Repositories.CoreState
{
    public class CoreRepository : ICoreRepository
    {
        private readonly CoreStateContext _coreStateContext;
        public CoreRepository()
        {
            _coreStateContext = new CoreStateContext();
        }

        public void AddComponent(Component component)
        {
            _coreStateContext
                .Components
                .Add(component);
        }

        public void AddProduct(Product product)
        {
            _coreStateContext
                .Products
                .Add(product);
        }

        public void AddSubscription(Subscription subscription)
        {
            _coreStateContext
                .Subscriptions
                .Add(subscription);
        }

        public void AddTenant(Tenant tenant)
        {
            _coreStateContext
                .Tenants
                .Add(tenant);
        }

        public void AddTopic(Topic topic)
        {
            _coreStateContext
                .Topics
                .Add(topic);
        }

        public void DeleteComponent(long componentId)
        {
            var component = GetComponent(componentId);

            if (component is not null)
                _coreStateContext.Components.Remove(component);
        }

        public void DeleteProduct(long productId)
        {
            var product = GetProduct(productId);

            if (product is not null)
                _coreStateContext.Products.Remove(product);
        }

        public void DeleteSubscription(long subscriptionId)
        {
            var subscription = GetSubscription(subscriptionId);

            if (subscription is not null)
                _coreStateContext.Subscriptions.Remove(subscription);
        }

        public void DeleteTenant(long tenantId)
        {
            var tenant = GetTenant(tenantId);

            if (tenant is not null)
                _coreStateContext.Tenants.Remove(tenant);
        }

        public void DeleteTopic(long topicId)
        {
            var topic = GetTopic(topicId);

            if (topic is not null)
                _coreStateContext.Topics.Remove(topic);
        }

        public void EditComponent(Component component)
        {
            _coreStateContext
                .Components
                .Update(component);
        }

        public void EditProduct(Product product)
        {
            _coreStateContext
                .Products
                .Update(product);
        }

        public void EditSubscription(Subscription subscription)
        {
            _coreStateContext
                .Subscriptions
                .Update(subscription);
        }

        public void EditTenant(Tenant tenant)
        {
            _coreStateContext
                .Tenants
                .Update(tenant);
        }

        public void EditTopic(Topic topic)
        {
            _coreStateContext
                .Topics
                .Update(topic);
        }

        public Component GetComponent(long componentId)
        {
            return _coreStateContext
                .Components
                .Find(componentId);
        }

        public Product GetProduct(long productId)
        {
            return _coreStateContext
                .Products
                .Find(productId);
        }

        public Subscription GetSubscription(long subscriptionId)
        {
            return _coreStateContext
                .Subscriptions
                .Find(subscriptionId);
        }

        public Tenant GetTenant(long tenantId)
        {
            return _coreStateContext
                .Tenants
                .Find(tenantId);
        }

        public Topic GetTopic(long topicId)
        {
            return _coreStateContext
                .Topics
                .Find(topicId);
        }

        public void SoftDeleteComponent(long componentId)
        {
            var componentSettings = GetComponentSettings(componentId);
            if (componentSettings is not null)
            {
                componentSettings.IsMarkedForDeletion = true;
                componentSettings.UpdatedDate = DateTimeOffset.Now;
                componentSettings.UpdatedBy = "SYSTEM";
                EditComponentSettings(componentSettings);
            }
        }

        public void SoftDeleteProduct(long productId)
        {
            var productSettings = GetProductSettings(productId);
            if (productSettings is not null)
            {
                productSettings.UpdatedDate = DateTimeOffset.Now;
                productSettings.UpdatedBy = "SYSTEM";
                productSettings.IsMarkedForDeletion = true;
                EditProductSettings(productSettings);
            }
        }

        public void SoftDeleteSubscription(long subscriptionId)
        {
            var subscription = GetSubscription(subscriptionId);

            if (subscription is not null)
            {
                subscription.IsMarkedForDeletion = true;
                EditSubscription(subscription);
            }
        }

        public void SoftDeleteTenant(long tenantId)
        {
            var tenantSettings = GetTenantSettings(tenantId);

            if (tenantSettings is not null)
            {
                tenantSettings.IsMarkedForDeletion = true;
                tenantSettings.UpdatedDate = DateTimeOffset.Now;
                tenantSettings.UpdatedBy = "SYSTEM";
                EditTenantSettings(tenantSettings);
            }
        }

        public void SoftDeleteTopic(long topicId)
        {
            var topic = GetTopic(topicId);

            if (topic is not null)
            {
                topic.IsMarkedForDeletion = true;
                topic.UpdatedDate = DateTimeOffset.Now;
                topic.UpdatedBy = "SYSTEM";
                EditTopic(topic);
            }
        }

        public void AddTenantSettings(TenantSettings tenantSettings)
        {
            _coreStateContext
                .TenantSettings
                .Add(tenantSettings);
        }

        public void EditTenantSettings(TenantSettings tenantSettings)
        {
            _coreStateContext
                .TenantSettings
                .Update(tenantSettings);
        }

        public void DeleteTenantSettings(long tenantId)
        {
            var tenantSettings = GetTenantSettings(tenantId);

            if (tenantSettings is not null)
                _coreStateContext.TenantSettings.Remove(tenantSettings);
        }

        public TenantSettings GetTenantSettings(long tenantId)
        {
            return _coreStateContext
                .TenantSettings
                .Where(t => t.TenantId == tenantId)
                .FirstOrDefault();
        }

        public void AddTenantToken(TenantToken tenantToken)
        {
            _coreStateContext
                .TenantTokens
                .Add(tenantToken);
        }

        public void EditTenantToken(TenantToken tenantToken)
        {
            _coreStateContext
                .TenantTokens
                .Update(tenantToken);
        }

        public void DeleteTenantToken(Guid id)
        {
            var tenantToken = GetTenantToken(id);

            if (tenantToken is not null)
                _coreStateContext.TenantTokens.Remove(tenantToken);

        }

        public TenantToken GetTenantToken(Guid id)
        {
            return _coreStateContext
                .TenantTokens
                .Find(id);
        }

        public void AddTenantRetention(TenantRetention tenantRetention)
        {
            _coreStateContext
                .TenantRetentions
                .Add(tenantRetention);
        }

        public void EditTenantRetention(TenantRetention tenantRetention)
        {
            _coreStateContext
                .TenantRetentions
                .Update(tenantRetention);
        }

        public void DeleteTenantRetention(long retentionId)
        {
            var tenantRetention = GetTenantRetention(retentionId);

            if (tenantRetention is not null)
                _coreStateContext.TenantRetentions.Remove(tenantRetention);
        }

        public TenantRetention GetTenantRetention(long retentionId)
        {
            return _coreStateContext
                 .TenantRetentions
                 .Find(retentionId);
        }

        public List<TenantRetention> GetTenantRetentions(long tenantId)
        {
            return _coreStateContext
                .TenantRetentions
                .Where(t => t.TenantId == tenantId)
                .ToList();
        }

        public void AddProductToken(ProductToken productToken)
        {
            _coreStateContext
                .ProductTokens
                .Add(productToken);
        }

        public void EditProductToken(ProductToken productToken)
        {
            _coreStateContext
                .ProductTokens
                .Update(productToken);
        }

        public void DeleteProductToken(Guid id)
        {
            var productToken = GetProductToken(id);

            if (productToken is not null)
                _coreStateContext.ProductTokens.Remove(productToken);
        }

        public ProductToken GetProductToken(Guid id)
        {
            return _coreStateContext
                .ProductTokens
                .Find(id);
        }

        public void AddProductSettings(ProductSettings productSettings)
        {
            _coreStateContext
                .ProductSettings
                .Add(productSettings);
        }

        public void EditProductSettings(ProductSettings productSettings)
        {
            _coreStateContext
                .ProductSettings
                .Update(productSettings);
        }

        public void DeleteProductSettings(long productId)
        {
            var productSettings = GetProductSettings(productId);

            if (productSettings is not null)
                _coreStateContext.ProductSettings.Remove(productSettings);
        }

        public ProductSettings GetProductSettings(long productId)
        {
            return _coreStateContext
                .ProductSettings
                .Where(p => p.ProductId == productId)
                .FirstOrDefault();
        }

        public void AddProductRetention(ProductRetention productRetention)
        {
            _coreStateContext
                .ProductRetentions
                .Add(productRetention);
        }

        public void EditProductRetention(ProductRetention productRetention)
        {
            _coreStateContext
                .ProductRetentions
                .Update(productRetention);
        }

        public void DeleteProductRetention(long retentionId)
        {
            var productRetention = GetProductRetention(retentionId);

            if (productRetention is not null)
                _coreStateContext.ProductRetentions.Remove(productRetention);
        }

        public ProductRetention GetProductRetention(long retentionId)
        {
            return _coreStateContext
                .ProductRetentions
                .Find(retentionId);
        }

        public List<ProductRetention> GetProductRetentions(long productId)
        {
            return _coreStateContext
                .ProductRetentions
                .Where(p => p.ProductId == productId)
                .ToList();
        }

        public void AddComponentToken(ComponentToken componentToken)
        {
            _coreStateContext
                .ComponentTokens
                .Add(componentToken);
        }

        public void EditComponentToken(ComponentToken componentToken)
        {
            _coreStateContext
                .ComponentTokens
                .Update(componentToken);
        }

        public void DeleteComponentToken(Guid id)
        {
            var componentToken = GetComponentToken(id);

            if (componentToken is not null)
                _coreStateContext.ComponentTokens.Remove(componentToken);
        }

        public ComponentToken GetComponentToken(Guid id)
        {
            return _coreStateContext
                .ComponentTokens
                .Find(id);
        }

        public void AddComponentSettings(ComponentSettings componentSettings)
        {
            _coreStateContext
                .ComponentSettings
                .Add(componentSettings);
        }

        public void EditComponentSettings(ComponentSettings componentSettings)
        {
            _coreStateContext
                .ComponentSettings
                .Update(componentSettings);
        }

        public void DeleteComponentSettings(long componentId)
        {
            var componentSettings = GetComponentSettings(componentId);

            if (componentSettings is not null)
                _coreStateContext.ComponentSettings.Remove(componentSettings);
        }

        public ComponentSettings GetComponentSettings(long componentId)
        {
            return _coreStateContext
                .ComponentSettings
                .Where(c => c.ComponentId == componentId)
                .FirstOrDefault();
        }

        public void AddComponentRetention(ComponentRetention componentRetention)
        {
            _coreStateContext
                .ComponentRetentions
                .Add(componentRetention);
        }

        public void EditComponentRetention(ComponentRetention componentRetention)
        {
            _coreStateContext
                .ComponentRetentions
                .Update(componentRetention);
        }

        public void DeleteComponentRetention(long retentionId)
        {
            var componentRetention = GetComponentRetention(retentionId);

            if (componentRetention is not null)
                _coreStateContext.ComponentRetentions.Remove(componentRetention);
        }

        public ComponentRetention GetComponentRetention(long retentionId)
        {
            return _coreStateContext
                .ComponentRetentions
                .Find(retentionId);
        }

        public List<ComponentRetention> GetComponentRetentions(long componentId)
        {
            return _coreStateContext
                .ComponentRetentions
                .Where(c => c.ComponentId == componentId)
                .ToList();
        }

        public Tenant GetTenant(string tenantName)
        {
            return _coreStateContext
                .Tenants
                .Where(t => t.Name == tenantName)
                .FirstOrDefault();
        }

        public int SaveChanges()
        {
            return _coreStateContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _coreStateContext.SaveChangesAsync();
        }

        public Product GetProduct(long tenantId, string productName)
        {
            return _coreStateContext
                .Products
                .Where(p => p.TenantId == tenantId && p.Name == productName)
                .FirstOrDefault();
        }

        public Component GetComponent(long tenantId, long productId, string componentName)
        {
            return _coreStateContext
                .Components
                .Where(c => c.ProductId == productId && c.Name == componentName)
                .FirstOrDefault();
        }

        public Topic GetTopic(long componentId, string topic)
        {
            return _coreStateContext
            .Topics
            .Where(c => c.ComponentId == componentId && c.Name == topic)
            .FirstOrDefault();
        }

        public Subscription GetSubscription(long topicId, string subscriptionName)
        {
            return _coreStateContext
              .Subscriptions
              .Where(c => c.TopicId == topicId && c.Name == subscriptionName)
              .FirstOrDefault();
        }

        public CoreStateContext GetCoreStateContext()
        {
            return _coreStateContext;
        }

        public List<Tenant> GetTenants(bool withInActive = true)
        {
            if (withInActive == true)
                return _coreStateContext
                    .Tenants
                    .ToList();
            else
                return _coreStateContext
                    .Tenants.Where(t => t.IsActive == true)
                    .ToList();
        }

        public List<Product> GetProducts(long tenantId)
        {
            return _coreStateContext
                .Products
                .Where(p => p.TenantId == tenantId)
                .ToList();
        }

        public List<Component> GetComponents(long productId)
        {
            return _coreStateContext
                .Components
                .Where(c => c.ProductId == productId)
                .ToList();
        }

        public List<Topic> GetTopics(long componentId)
        {
            return _coreStateContext
                .Topics
                .Where(t => t.ComponentId == componentId)
                .ToList();
        }

        public List<Subscription> GetSubscriptions(long topicId)
        {
            return _coreStateContext
                .Subscriptions
                .Where(s => s.TopicId == topicId)
                .ToList();
        }

        public List<TenantToken> GetTenantToken(long tenantId)
        {
            return _coreStateContext
                .TenantTokens
                .Where(t => t.TenantId == tenantId)
                .ToList();
        }

        public List<ProductToken> GetProductToken(long productId)
        {
            return _coreStateContext
             .ProductTokens
             .Where(t => t.ProductId == productId)
             .ToList();
        }

        public List<ComponentToken> GetComponentToken(long componentId)
        {
            return _coreStateContext
                .ComponentTokens
                .Where(t => t.ComponentId == componentId)
                .ToList();
        }

        public void AddTopicSettings(TopicSettings topicSettings)
        {
            _coreStateContext
                .TopicSettings
                .Add(topicSettings);
        }

        public void EditTopicSettings(TopicSettings topicSettings)
        {

            _coreStateContext
                .TopicSettings
                .Update(topicSettings);

        }

        public void DeleteTopicSettings(long topicId)
        {
            var topicSettings = GetTopicSettings(topicId);
            if (topicSettings is null)
                return;

            _coreStateContext.TopicSettings.Remove(null);
        }

        public TopicSettings GetTopicSettings(long topicId)
        {
            return _coreStateContext
                .TopicSettings
                .Where(t => t.TopicId == topicId)
                .FirstOrDefault();
        }

        public TopicSettings GetTopicSettings(string tenant, string product, string component, string topic)
        {
            var tenantDetails = GetTenant(tenant);
            var productDetails = GetProduct(tenantDetails.Id, product);
            var componentDetails = GetComponent(tenantDetails.Id, productDetails.Id, component);
            var topicDetails = GetTopic(componentDetails.Id, topic);


            return GetTopicSettings(topicDetails.Id);
        }
    }
}
