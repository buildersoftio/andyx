using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Repositories;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.IO.Services;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Core.Services.App
{
    public class TenantStateService : ITenantStateService
    {
        private readonly ILogger<TenantStateService> _logger;
        private readonly ITenantStateRepository _tenantStateRepository;

        private readonly ITenantFactory _tenantFactory;
        private readonly ICoreRepository _coreRepository;
        private readonly ICoreService _coreService;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ISubscriptionHubRepository _subscriptionHubRepository;
        private readonly IOutboundMessageService _outboundMessageService;
        private readonly ISubscriptionFactory _subscriptionFactory;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly IClusterHubService _clusterHubService;
        private readonly IInboundMessageService _inboundMessageService;

        public TenantStateService(ILogger<TenantStateService> logger,
            ITenantStateRepository tenantStateRepository,
            ITenantFactory tenantFactory,
            ICoreRepository coreRepository,
            ICoreService coreService,
            IOrchestratorService orchestratorService,
            ISubscriptionHubRepository subscriptionHubRepository,
            IOutboundMessageService outboundMessageService,
            ISubscriptionFactory subscriptionFactory,
            NodeConfiguration nodeConfiguration,
            IClusterHubService clusterHubService,
            IInboundMessageService inboundMessageService)
        {
            _logger = logger;
            _tenantStateRepository = tenantStateRepository;

            _tenantFactory = tenantFactory;
            _coreRepository = coreRepository;
            _coreService = coreService;
            _orchestratorService = orchestratorService;
            _subscriptionHubRepository = subscriptionHubRepository;
            _outboundMessageService = outboundMessageService;
            _subscriptionFactory = subscriptionFactory;
            _nodeConfiguration = nodeConfiguration;
            _clusterHubService = clusterHubService;
            _inboundMessageService = inboundMessageService;

            AddTenantsFromPersistentLog();
        }

        private void AddTenantsFromPersistentLog()
        {
            var tenants = _coreRepository.GetTenants(withInActive: false);
            foreach (var tenant in tenants)
            {
                // check if tenant location exits.
                var tenantSettings = _coreRepository.GetTenantSettings(tenant.Id);
                var tenantToAddInMemory = _tenantFactory
                    .CreateTenant(tenant.Name, tenantSettings);

                AddTenant(tenant.Name, tenantToAddInMemory);
                var products = _coreRepository.GetProducts(tenant.Id);
                foreach (var product in products)
                {
                    AddProduct(tenant.Name, product.Name, _tenantFactory.CreateProduct(product.Name));

                    var components = _coreRepository.GetComponents(product.Id);
                    foreach (var component in components)
                    {
                        var componentSettings = _coreRepository.GetComponentSettings(component.Id);
                        AddComponent(tenant.Name, product.Name, component.Name, _tenantFactory.CreateComponent(component.Name, component.Description, componentSettings));

                        var topics = _coreRepository.GetTopics(component.Id);
                        foreach (var topic in topics)
                        {
                            AddTopic(tenant.Name, product.Name, component.Name, topic.Name, _tenantFactory.CreateTopic(topic.Name, topic.Description));

                            var subscriptions = _coreRepository.GetSubscriptions(topic.Id);
                            foreach (var subscription in subscriptions)
                            {
                                AddSubscriptionConfiguration(tenant.Name, product.Name, component.Name, topic.Name, subscription.Name,
                                        _subscriptionFactory.CreateSubscription(tenant.Name, product.Name, component.Name, topic.Name, subscription.Name, subscription.SubscriptionType,
                                        subscription.SubscriptionMode, subscription.InitialPosition));
                            }
                        }
                    }
                }
            }
        }

        public bool AddTopic(string tenant, string product, string component, string topicName, Topic topic, bool notifyOtherNodes = true)
        {
            if (_tenantStateRepository.GetTenants().ContainsKey(tenant))
                if (_tenantStateRepository.GetTenants()[tenant].Products.ContainsKey(product))
                    if (_tenantStateRepository.GetTenants()[tenant].Products[product].Components.ContainsKey(component))
                    {
                        _tenantStateRepository.GetTenants()[tenant].Products[product].Components[component].Topics.TryAdd(topicName, topic);
                    }

            var tenantDetail = _coreRepository.GetTenant(tenant);
            if (tenantDetail != null)
            {
                var productDetail = _coreRepository.GetProduct(tenantDetail.Id, product);
                if (productDetail == null)
                    return false;

                var componentDetails = _coreRepository.GetComponent(tenantDetail.Id, productDetail.Id, component);
                if (componentDetails == null)
                    return false;

                if (TenantIOService.TryCreateTopicDirectory(tenant, product, component, topicName) == true)
                {
                    if (notifyOtherNodes == true)
                        _clusterHubService.CreateTopic_AllNodes(tenant, product, component, topic);
                }

                // Open connection with topic log data.
                using (var topicStateContext = new TopicEntryPositionContext(tenant, product, component, topicName))
                {
                    topicStateContext.Database.EnsureCreated();
                    var currentData = topicStateContext.TopicStates.Find(_nodeConfiguration.NodeId);
                    if (currentData == null)
                    {
                        currentData = new Model.Entities.Storages.TopicEntryPosition()
                        {
                            Id = _nodeConfiguration.NodeId,
                            CurrentEntry = 1,
                            MarkDeleteEntryPosition = 0,

                            CreateDate = System.DateTimeOffset.Now
                        };

                        topicStateContext.TopicStates.Add(currentData);
                        topicStateContext.SaveChanges();
                    }
                    topic.TopicStates.LatestEntryId = currentData.CurrentEntry;
                    topic.TopicStates.MarkDeleteEntryPosition = currentData.MarkDeleteEntryPosition;
                }

                string topicKey = ConnectorHelper.GetTopicConnectorKey(tenant, product, component, topicName);
                _inboundMessageService.TryCreateTopicConnector(topicKey, 1);

                var isCreated = _coreService.CreateTopic(tenant, product, component, topicName, topic.Description);

                _orchestratorService.InitializeTopicDataService(tenant, product, component, topic);

                // We are not initializing the readonly when the topic is created, beacuse of memory leak.
                //_orchestratorService.InitializeTopicReadonlyDataService(tenant, product, component, topic);

                var topicDetails = _coreRepository.GetTopic(componentDetails.Id, topicName);
                if (topicDetails != null)
                    return false;

                return isCreated;
            }

            return false;
        }

        public bool AddSubscriptionConfiguration(string tenant, string product, string component, string topicName, string subscriptionName, Subscription subscription, bool notifyOtherNodes = true)
        {

            var tenantDetail = _coreRepository.GetTenant(tenant);
            if (tenantDetail != null)
            {
                var productDetail = _coreRepository.GetProduct(tenantDetail.Id, product);
                if (productDetail == null)
                    return false;

                var componentDetails = _coreRepository.GetComponent(tenantDetail.Id, productDetail.Id, component);
                if (componentDetails == null)
                    return false;

                var topicDetails = _coreRepository.GetTopic(componentDetails.Id, topicName);
                if (topicDetails == null)
                    return false;

                if (TenantIOService.TryCreateSubscriptionDirectory(tenant, product, component, topicName, subscriptionName) == true)
                {
                    if (notifyOtherNodes == true)
                        _clusterHubService.CreateSubscription_AllNodes(subscription);
                }

                var subId = ConnectorHelper.GetSubcriptionId(tenant, product, component, topicName, subscriptionName);
                _subscriptionHubRepository.AddSubscription(subId, _tenantStateRepository.GetTenants()[tenant].Products[product].Components[component].Topics[topicName], subscription);


                // check if the subscription exists in topicState
                var nodeSubscriptionId = ConnectorHelper.GetNodeSubcriptionId(_nodeConfiguration.NodeId, subId);
                using (var topicStateContext = new TopicEntryPositionContext(tenant, product, component, topicName))
                {
                    var currentSubscriptionData = topicStateContext.TopicStates.Find(nodeSubscriptionId);
                    if (currentSubscriptionData == null)
                    {
                        currentSubscriptionData = new Model.Entities.Storages.TopicEntryPosition()
                        {
                            Id = nodeSubscriptionId,
                            CurrentEntry = 0,
                            MarkDeleteEntryPosition = 0,
                            CreateDate = System.DateTimeOffset.Now
                        };
                        topicStateContext.TopicStates.Add(currentSubscriptionData);
                        topicStateContext.SaveChanges();
                    }
                }

                _orchestratorService.InitializeSubscriptionUnackedDataService(tenant, product, component, topicName, subscriptionName);

                var subscriptionDetails = _coreRepository.GetSubscription(topicDetails.Id, subscriptionName);
                if (subscriptionDetails == null)
                {
                    return _coreService.CreateSubscription(tenant, product, component, topicName, subscriptionName, subscription.SubscriptionType, subscription.SubscriptionMode, subscription.InitialPosition);
                }
            }

            return true;
        }


        public bool AddComponent(string tenant, string product, string componentName, Component component, bool notifyOtherNodes = true)
        {
            if (_tenantStateRepository.GetTenants().ContainsKey(tenant))
                if (_tenantStateRepository.GetTenants()[tenant].Products.ContainsKey(product))
                    _tenantStateRepository.GetTenants()[tenant].Products[product].Components.TryAdd(componentName, component);

            //add component to tenants log

            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails != null)
            {
                var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
                if (productDetails == null)
                    return false;

                if (TenantIOService.TryCreateComponentDirectory(tenant, product, componentName) == true)
                {
                    if (notifyOtherNodes == true)
                        _clusterHubService.CreateComponent_AllNodes(tenant, product, component);
                }

                var componentDetails = _coreRepository.GetComponent(tenantDetails.Id, productDetails.Id, componentName);
                if (componentDetails != null)
                    return true;


                return _coreService.CreateComponent(tenant, product, componentName, component.Description,
                    component.Settings.IsTopicAutomaticCreationAllowed, component.Settings.EnforceSchemaValidation,
                    component.Settings.IsAuthorizationEnabled, component.Settings.IsSubscriptionAutomaticCreationAllowed,
                    component.Settings.IsProducerAutomaticCreationAllowed);
            }

            return false;
        }

        public bool AddProduct(string tenant, string productName, Product product, bool notifyOtherNodes = true)
        {
            if (_tenantStateRepository.GetTenants().ContainsKey(tenant))
                _tenantStateRepository.GetTenants()[tenant].Products.TryAdd(productName, product);

            // adding product to tenants_config
            var tenantDetail = _coreRepository.GetTenant(tenant);
            if (tenantDetail != null)
            {
                if (TenantIOService.TryCreateProductDirectory(tenant, productName) == true)
                {
                    if (notifyOtherNodes == true)
                        _clusterHubService.CreateProduct_AllNodes(tenant, product);
                }

                var productDetails = _coreRepository.GetProduct(tenantDetail.Id, productName);
                if (productDetails != null)
                    return true;

                return _coreService.CreateProduct(tenant, productName, product.Description);
            }

            return false;
        }

        public bool AddTenant(string tenantName, Tenant tenant, bool notifyOtherNodes = true)
        {
            if (TenantIOService.TryCreateTenantDirectory(tenantName) == true)
            {
                if (notifyOtherNodes == true)
                    _clusterHubService.CreateTenant_AllNodes(tenantName, tenant.Settings);
            }

            return _tenantStateRepository.GetTenants().TryAdd(tenantName, tenant);
        }

        public Component GetComponent(string tenant, string product, string component)
        {
            return _tenantStateRepository.GetComponent(tenant, product, component);
        }

        public ConcurrentDictionary<string, Component> GetComponents(string tenant, string product)
        {
            return _tenantStateRepository.GetComponents(tenant, product);
        }

        public Product GetProduct(string tenant, string product)
        {
            return _tenantStateRepository.GetProduct(tenant, product);
        }

        public ConcurrentDictionary<string, Product> GetProducts(string tenant)
        {
            return _tenantStateRepository.GetProducts(tenant);
        }



        public Tenant GetTenant(string tenant)
        {
            return _tenantStateRepository.GetTenant(tenant);
        }

        public ConcurrentDictionary<string, Tenant> GetTenants()
        {
            return _tenantStateRepository.GetTenants();
        }


        public Topic GetTopic(string tenant, string product, string component, string topic)
        {
            return _tenantStateRepository.GetTopic(tenant, product, component, topic);
        }

        public ConcurrentDictionary<string, Topic> GetTopics(string tenant, string product, string component)
        {
            return _tenantStateRepository.GetTopics(tenant, product, component);
        }
    }
}
