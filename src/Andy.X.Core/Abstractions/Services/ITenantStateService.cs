using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Subscriptions;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Core.Abstractions.Services
{
    public interface ITenantStateService
    {
        bool AddTenant(string tenantName, Tenant tenant, bool notifyOtherNodes = true);
        bool AddProduct(string tenant, string productName, Product product, bool notifyOtherNodes = true);
        bool AddComponent(string tenant, string product, string componentName, Component component, bool notifyOtherNodes = true);
        bool AddTopic(string tenant, string product, string component, string topicName, Topic topic, bool notifyOtherNodes = true);

        public bool AddSubscriptionConfiguration(string tenant, string product, string component, string topicName, string subscriptionName, Subscription subscription, bool notifyOtherNodes = true);


        Tenant GetTenant(string tenant);
        Product GetProduct(string tenant, string product);
        Component GetComponent(string tenant, string product, string component);
        Topic GetTopic(string tenant, string product, string component, string topic);

        ConcurrentDictionary<string, Tenant> GetTenants();
        ConcurrentDictionary<string, Product> GetProducts(string tenant);
        ConcurrentDictionary<string, Component> GetComponents(string tenant, string product);
        ConcurrentDictionary<string, Topic> GetTopics(string tenant, string product, string component);
    }
}
