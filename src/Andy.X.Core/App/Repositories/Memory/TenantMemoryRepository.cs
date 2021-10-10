using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.App.Repositories.Memory
{
    public class TenantMemoryRepository : ITenantRepository
    {
        private readonly ILogger<TenantMemoryRepository> logger;
        private readonly ITenantFactory tenantFactory;
        private ConcurrentDictionary<string, Model.App.Tenants.Tenant> _tenants;

        public TenantMemoryRepository(ILogger<TenantMemoryRepository> logger, List<TenantConfiguration> tenantConfigurations, ITenantFactory tenantFactory)
        {
            this.logger = logger;
            this.tenantFactory = tenantFactory;

            _tenants = new ConcurrentDictionary<string, Model.App.Tenants.Tenant>();

            AddTenantsFromConfiguration(tenantConfigurations);
        }


        private void AddTenantsFromConfiguration(List<TenantConfiguration> tenantConfigurations)
        {
            foreach (var tenantConfig in tenantConfigurations)
            {
                AddTenant(tenantConfig.Name, tenantFactory.CreateTenant(tenantConfig.Name, tenantConfig.DigitalSignature));
            }
        }


        public bool AddTopic(string tenant, string product, string component, string topicName, Topic topic)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    if (_tenants[tenant].Products[product].Components.ContainsKey(component))
                        _tenants[tenant].Products[product].Components[component].Topics.TryAdd(topicName, topic);

            return false;
        }

        public bool AddComponent(string tenant, string product, string componentName, Component component)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    _tenants[tenant].Products[product].Components.TryAdd(componentName, component);

            return false;
        }

        public bool AddProduct(string tenant, string productName, Product product)
        {
            if (_tenants.ContainsKey(tenant))
                return _tenants[tenant].Products.TryAdd(productName, product);

            return false;
        }

        public bool AddTenant(string tenantName, Model.App.Tenants.Tenant tenant)
        {
            return _tenants.TryAdd(tenantName, tenant);
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



        public Model.App.Tenants.Tenant GetTenant(string tenant)
        {
            if (_tenants.ContainsKey(tenant))
                return _tenants[tenant];

            return null;
        }

        public ConcurrentDictionary<string, Model.App.Tenants.Tenant> GetTenants()
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
