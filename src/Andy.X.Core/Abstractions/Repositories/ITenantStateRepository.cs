using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories
{
    public interface ITenantStateRepository
    {
        Tenant GetTenant(string tenant);
        ConcurrentDictionary<string, Tenant> GetTenants();

        Product GetProduct(string tenant, string product);
        ConcurrentDictionary<string, Product> GetProducts(string tenant);

        Component GetComponent(string tenant, string product, string component);
        ConcurrentDictionary<string, Component> GetComponents(string tenant, string product);

        public Topic GetTopic(string tenant, string product, string component, string topic);
        public ConcurrentDictionary<string, Topic> GetTopics(string tenant, string product, string component);
    }
}
