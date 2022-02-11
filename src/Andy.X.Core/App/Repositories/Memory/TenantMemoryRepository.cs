using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
                AddTenantFromApi(tenantConfig);
            }
        }

        public void AddTenantFromApi(TenantConfiguration tenantConfig)
        {
            AddTenant(tenantConfig.Name, tenantFactory
                   .CreateTenant(tenantConfig.Name,
                       tenantConfig.Settings.DigitalSignature,
                       tenantConfig.Settings.EnableEncryption,
                       tenantConfig.Settings.AllowProductCreation,
                       tenantConfig.Settings.EnableAuthorization,
                       tenantConfig.Settings.Tokens,
                       tenantConfig.Settings.Logging,
                       tenantConfig.Settings.EnableGeoReplication,
                       tenantConfig.Settings.CertificatePath));

            // add products
            tenantConfig.Products.ForEach(product =>
            {
                AddProduct(tenantConfig.Name, product.Name, tenantFactory.CreateProduct(product.Name));

                // add components of product
                product.Components.ForEach(component =>
                {
                    AddComponent(tenantConfig.Name,
                        product.Name,
                        component.Name,
                        tenantFactory.CreateComponent(component.Name,
                            component.Settings.AllowSchemaValidation,
                            component.Settings.AllowTopicCreation,
                            component.Settings.EnableAuthorization,
                            component.Settings.Tokens));

                    // Add topics from configuration

                    component.Topics.ForEach(topic =>
                    {
                        AddTopic(tenantConfig.Name, product.Name, component.Name, topic.Name, tenantFactory.CreateTopic(topic.Name));
                    });
                });
            });
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



        public Tenant GetTenant(string tenant)
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

        public TenantToken GetTenantToken(string tenant, string token)
        {
            if (_tenants.ContainsKey(tenant))
                return _tenants[tenant].Settings.Tokens.Where(t => t.Token == token).FirstOrDefault();

            return null;
        }

        public ComponentToken GetComponentToken(string tenant, string product, string component, string componentToken)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    if (_tenants[tenant].Products[product].Components.ContainsKey(component))
                        return _tenants[tenant].Products[product].Components[component]
                            .Settings
                            .Tokens
                            .Where(t => t.Token == componentToken).FirstOrDefault();

            return null;
        }

        public bool AddTenantToken(string tenant, TenantToken token)
        {
            var tenantDetails = GetTenant(tenant);
            if (tenantDetails == null)
                return false;

            tenantDetails.Settings.Tokens.Add(token);
            return true;
        }

        public bool AddComponentToken(string tenant, string product, string component, ComponentToken componentToken)
        {
            var componentDetail = GetComponent(tenant, product, component);
            if (componentDetail == null)
                return false;

            componentDetail.Settings.Tokens.Add(componentToken);
            return true;
        }

        public List<ComponentToken> GetComponentTokens(string tenant, string product, string component)
        {
            var componentDetail = GetComponent(tenant, product, component);
            if (componentDetail == null)
                return new List<ComponentToken>();

            return componentDetail.Settings.Tokens;
        }

        public bool AddComponentRetention(string tenant, string product, string component, ComponentRetention componentRetention)
        {
            var componentDetail = GetComponent(tenant, product, component);
            if (componentDetail == null)
                return false;

            componentDetail.Settings.RetentionPolicy = componentRetention;
            return true;
        }

        public ComponentRetention GetComponentRetention(string tenant, string product, string component)
        {
            var componentDetail = GetComponent(tenant, product, component);
            if (componentDetail == null)
                return null;

            return componentDetail.Settings.RetentionPolicy;
        }

        public bool RemoveTenantToken(string tenant, string token)
        {
            var tenantDetails = GetTenant(tenant);
            if (tenantDetails == null)
                return false;

            var tenantToken = tenantDetails.Settings.Tokens.Find(x => x.Token == token);
            if (tenantToken == null)
                return true;

            return tenantDetails.Settings.Tokens.Remove(tenantToken);
        }

        public bool RemoveComponentToken(string tenant, string product, string component, string token)
        {
            var componentDetail = GetComponent(tenant, product, component);
            if (componentDetail == null)
                return false;
            var componentToken = componentDetail.Settings.Tokens.Find(x => x.Token == token);
            if (componentToken == null)
                return true;

            return componentDetail.Settings.Tokens.Remove(componentToken);
        }
    }
}
