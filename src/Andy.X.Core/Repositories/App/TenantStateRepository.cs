using Buildersoft.Andy.X.Core.Abstractions.Repositories;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Core.Repositories.App
{
    public class TenantStateRepository: ITenantStateRepository
    {
        private readonly ConcurrentDictionary<string, Tenant> _tenants;

        public TenantStateRepository()
        {
            _tenants = new ConcurrentDictionary<string, Tenant>();
        }

        public Component GetComponent(string tenant, string product, string component)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    if (_tenants[tenant].Products[product].Components.ContainsKey(component))
                        return _tenants[tenant].Products[product].Components[component];

            return null;
        }

        public ConcurrentDictionary<string, Component> GetComponents(string tenant, string product)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    return _tenants[tenant].Products[product].Components;

            return null;
        }

        public Product GetProduct(string tenant, string product)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    return _tenants[tenant].Products[product];

            return null;
        }

        public ConcurrentDictionary<string, Product> GetProducts(string tenant)
        {
            if (_tenants.ContainsKey(tenant))
                return _tenants[tenant].Products;

            return null;
        }



        public Tenant GetTenant(string tenant)
        {
            if (_tenants.ContainsKey(tenant))
                return _tenants[tenant];

            return null;
        }

        public ConcurrentDictionary<string, Tenant> GetTenants()
        {
            return _tenants;
        }


        public Topic GetTopic(string tenant, string product, string component, string topic)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    if (_tenants[tenant].Products[product].Components.ContainsKey(component))
                        if (_tenants[tenant].Products[product].Components[component].Topics.ContainsKey(topic))
                            return _tenants[tenant].Products[product].Components[component].Topics[topic];

            return null;
        }

        public ConcurrentDictionary<string, Topic> GetTopics(string tenant, string product, string component)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    if (_tenants[tenant].Products[product].Components.ContainsKey(component))
                        return _tenants[tenant].Products[product].Components[component].Topics;

            return null;
        }
    }
}
