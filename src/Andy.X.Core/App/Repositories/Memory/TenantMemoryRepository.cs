using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.IO.Readers;
using Buildersoft.Andy.X.IO.Services;
using Buildersoft.Andy.X.IO.Writers;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Buildersoft.Andy.X.Core.App.Repositories.Memory
{
    public class TenantMemoryRepository : ITenantRepository
    {
        private readonly ILogger<TenantMemoryRepository> _logger;
        private readonly ITenantFactory _tenantFactory;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ISubscriptionHubRepository _subscriptionHubRepository;
        private readonly IOutboundMessageService _outboundMessageService;
        private readonly ISubscriptionFactory _subscriptionFactory;

        private readonly ConcurrentDictionary<string, Tenant> _tenants;

        public TenantMemoryRepository(ILogger<TenantMemoryRepository> logger,
            List<TenantConfiguration> tenantConfigurations,
            ITenantFactory tenantFactory,
            IOrchestratorService orchestratorService,
            ISubscriptionHubRepository subscriptionHubRepository,
            IOutboundMessageService outboundMessageService,
            ISubscriptionFactory subscriptionFactory)
        {
            _logger = logger;
            _tenantFactory = tenantFactory;
            _orchestratorService = orchestratorService;
            _subscriptionHubRepository = subscriptionHubRepository;
            _outboundMessageService = outboundMessageService;
            _subscriptionFactory = subscriptionFactory;

            _tenants = new ConcurrentDictionary<string, Tenant>();

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
            var tenantDetails = _tenantFactory
                   .CreateTenant(tenantConfig.Name,
                       tenantConfig.Settings.DigitalSignature,
                       tenantConfig.Settings.EnableEncryption,
                       tenantConfig.Settings.AllowProductCreation,
                       tenantConfig.Settings.EnableAuthorization,
                       tenantConfig.Settings.Tokens,
                       tenantConfig.Settings.Logging,
                       tenantConfig.Settings.EnableGeoReplication,
                       tenantConfig.Settings.CertificatePath);
            AddTenant(tenantConfig.Name, tenantDetails);

            // add products
            tenantConfig.Products.ForEach(product =>
            {
                AddProduct(tenantConfig.Name, product.Name, _tenantFactory.CreateProduct(product.Name));
                // add components of product
                product.Components.ForEach(component =>
                {
                    AddComponent(tenantConfig.Name,
                        product.Name,
                        component.Name,
                        _tenantFactory.CreateComponent(component.Name,
                            component.Settings.AllowSchemaValidation,
                            component.Settings.AllowTopicCreation,
                            component.Settings.EnableAuthorization,
                            component.Settings.Tokens));
                    // Add topics from configuration

                    component.Topics.ForEach(topic =>
                    {
                        AddTopic(tenantConfig.Name, product.Name, component.Name, topic.Name, _tenantFactory.CreateTopic(topic.Name));

                        foreach (var subscription in topic.Subscriptions)
                        {
                            AddSubscriptionConfiguration(tenantConfig.Name, product.Name, component.Name, topic.Name, subscription.Key
                                , _subscriptionFactory.CreateSubscription(tenantConfig.Name, product.Name, component.Name, topic.Name, subscription.Key, subscription.Value.SubscriptionType,
                                subscription.Value.SubscriptionMode, subscription.Value.InitialPosition));
                        }
                    });
                });
            });
        }

        public bool AddTopic(string tenant, string product, string component, string topicName, Topic topic)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    if (_tenants[tenant].Products[product].Components.ContainsKey(component))
                    {
                        _tenants[tenant].Products[product].Components[component].Topics.TryAdd(topicName, topic);
                    }

            List<TenantConfiguration> tenantsConfig = TenantIOReader.ReadTenantsFromConfigFile();
            var tenantDetail = tenantsConfig.Find(x => x.Name == tenant);
            if (tenantDetail != null)
            {
                var productDetail = tenantDetail.Products.Find(x => x.Name == product);
                if (productDetail == null)
                    return false;

                var componentDetails = productDetail.Components.Find(x => x.Name == component);
                if (componentDetails == null)
                    return false;

                TenantIOService.TryCreateTopicDirectory(tenant, product, component, topicName);

                // Open connection with topic log data.
                using (var topicStateContext = new TopicStateContext(tenant, product, component, topicName))
                {
                    topicStateContext.Database.EnsureCreated();
                    var currentData = topicStateContext.TopicStates.Find("DEFAULT");
                    if (currentData == null)
                    {
                        currentData = new Model.Entities.Storages.TopicState()
                        {
                            Id = "DEFAULT",
                            CurrentEntry = 1,
                            MarkDeleteEntryPosition = -1,
                            CreateDate = System.DateTimeOffset.Now
                        };
                        topicStateContext.TopicStates.Add(currentData);
                        topicStateContext.SaveChanges();
                    }
                    topic.TopicStates.LatestEntryId = currentData.CurrentEntry;
                    topic.TopicStates.MarkDeleteEntryPosition = currentData.MarkDeleteEntryPosition;
                }

                _orchestratorService.InitializeTopicDataService(tenant, product, component, topic);

                var topicDetails = componentDetails.Topics.Find(x => x.Name == topicName);
                if (topicDetails != null)
                    return false;

                componentDetails.Topics.Add(new TopicConfiguration() { Name = topicName });
                if (TenantIOWriter.WriteTenantsConfiguration(tenantsConfig) == true)
                    return true;
            }

            return false;
        }

        public bool AddSubscriptionConfiguration(string tenant, string product, string component, string topicName, string subscriptionName, Subscription subscription)
        {

            List<TenantConfiguration> tenantsConfig = TenantIOReader.ReadTenantsFromConfigFile();
            var tenantDetail = tenantsConfig.Find(x => x.Name == tenant);
            if (tenantDetail != null)
            {
                var productDetail = tenantDetail.Products.Find(x => x.Name == product);
                if (productDetail == null)
                    return false;

                var componentDetails = productDetail.Components.Find(x => x.Name == component);
                if (componentDetails == null)
                    return false;

                var topicDetails = componentDetails.Topics.Find(x => x.Name == topicName);
                if (topicDetails == null)
                    return false;

                TenantIOService.TryCreateSubscriptionDirectory(tenant, product, component, topicName, subscriptionName);

                var subId = ConnectorHelper.GetSubcriptionId(tenant, product, component, topicName, subscriptionName);
                _subscriptionHubRepository.AddSubscription(subId, subscription);

                //load subscriptionopicData
                //_outboundMessageService.LoadSubscriptionTopicDataInMemory(_subscriptionFactory.CreateSubscriptionTopicData(subscription));

                if (topicDetails.Subscriptions.ContainsKey(subscriptionName) != true)
                {
                    topicDetails.Subscriptions.Add(subscriptionName, new SubscriptionConfiguration()
                    {
                        SubscriptionType = subscription.SubscriptionType,
                        SubscriptionMode = subscription.SubscriptionMode,
                        InitialPosition = subscription.InitialPosition,
                        CreatedDate = subscription.CreatedDate,
                    });

                    if (TenantIOWriter.WriteTenantsConfiguration(tenantsConfig) == true)
                        return true;
                }
            }

            return true;
        }


        public bool AddComponent(string tenant, string product, string componentName, Component component)
        {
            if (_tenants.ContainsKey(tenant))
                if (_tenants[tenant].Products.ContainsKey(product))
                    _tenants[tenant].Products[product].Components.TryAdd(componentName, component);

            //add component to tenants_config.json
            List<TenantConfiguration> tenantsConfig = TenantIOReader.ReadTenantsFromConfigFile();
            var tenantDetail = tenantsConfig.Find(x => x.Name == tenant);
            if (tenantDetail != null)
            {
                var productDetail = tenantDetail.Products.Find(x => x.Name == product);
                if (productDetail == null)
                    return false;

                TenantIOService.TryCreateComponentDirectory(tenant, product, componentName);

                var componentDetails = productDetail.Components.Find(x => x.Name == componentName);
                if (componentDetails != null)
                    return true;


                productDetail.Components.Add(new ComponentConfiguration() { Name = componentName, Settings = component.Settings, Topics = new List<TopicConfiguration>() });
                if (TenantIOWriter.WriteTenantsConfiguration(tenantsConfig) == true)
                    return true;
            }

            return false;
        }

        public bool AddProduct(string tenant, string productName, Product product)
        {
            if (_tenants.ContainsKey(tenant))
                _tenants[tenant].Products.TryAdd(productName, product);

            // adding product to tenants_config
            List<TenantConfiguration> tenantsConfig = TenantIOReader.ReadTenantsFromConfigFile();
            var tenantDetail = tenantsConfig.Find(x => x.Name == tenant);
            if (tenantDetail != null)
            {
                if (tenantDetail.Products == null)
                    tenantDetail.Products = new List<ProductConfiguration>();

                TenantIOService.TryCreateProductDirectory(tenant, productName);

                var productDetail = tenantDetail.Products.Find(x => x.Name == productName);
                if (productDetail != null)
                    return true;

                // register product to tenantConfiguration
                tenantDetail.Products.Add(new ProductConfiguration() { Name = productName, Components = new List<ComponentConfiguration>() });
                if (TenantIOWriter.WriteTenantsConfiguration(tenantsConfig) == true)
                    return true;
            }

            return false;
        }

        public bool AddTenant(string tenantName, Tenant tenant)
        {
            TenantIOService.TryCreateTenantDirectory(tenantName);
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
