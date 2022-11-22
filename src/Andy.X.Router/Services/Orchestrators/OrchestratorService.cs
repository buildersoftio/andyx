using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Services.Producers;
using Buildersoft.Andy.X.Model.App.Topics;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.Core.Services.Data;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Repositories;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Entities.Clusters;

namespace Buildersoft.Andy.X.Router.Services.Orchestrators
{
    public class OrchestratorService : IOrchestratorService
    {
        private readonly ILogger<OrchestratorService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IProducerHubService _producerHubService;
        private readonly StorageConfiguration _storageConfiguration;
        private readonly ICoreRepository _coreRepository;
        private readonly ITenantStateRepository _tenantStateRepository;

        private readonly ConcurrentDictionary<string, ITopicDataService<Message>> _topicDataServices;
        private readonly ConcurrentDictionary<string, ITopicReadonlyDataService<Message>> _topicReadonlyDataServices;

        private readonly ConcurrentDictionary<string, ITopicDataService<UnacknowledgedMessage>> _subscriptionUnackedDataServices;
        private readonly ConcurrentDictionary<string, ITopicReadonlyDataService<UnacknowledgedMessage>> _subscriptionUnackedReadonlyDataServices;

        private readonly ConcurrentDictionary<string, ITopicDataService<ClusterChangeLog>> _nodesDataServices;

        public OrchestratorService(
            ILoggerFactory logger,
            IProducerHubService producerHubService,
            StorageConfiguration storageConfiguration,
            ICoreRepository coreRepository,
            ITenantStateRepository tenantStateRepository)
        {
            _logger = logger.CreateLogger<OrchestratorService>();

            _loggerFactory = logger;
            _producerHubService = producerHubService;
            _storageConfiguration = storageConfiguration;
            _coreRepository = coreRepository;
            _tenantStateRepository = tenantStateRepository;

            _topicDataServices = new ConcurrentDictionary<string, ITopicDataService<Message>>();
            _topicReadonlyDataServices = new ConcurrentDictionary<string, ITopicReadonlyDataService<Message>>();

            _subscriptionUnackedDataServices = new ConcurrentDictionary<string, ITopicDataService<UnacknowledgedMessage>>();
            _subscriptionUnackedReadonlyDataServices = new ConcurrentDictionary<string, ITopicReadonlyDataService<UnacknowledgedMessage>>();

            _nodesDataServices = new ConcurrentDictionary<string, ITopicDataService<ClusterChangeLog>>();
        }

        public ITopicDataService<Message> GetTopicDataService(string topicKey)
        {
            if (_topicDataServices.ContainsKey(topicKey) != true)
                return null;

            return _topicDataServices[topicKey];
        }

        public ITopicReadonlyDataService<Message> GetTopicReadonlyDataService(string topicKey)
        {
            if (_topicReadonlyDataServices.ContainsKey(topicKey) != true)
                return null;

            return _topicReadonlyDataServices[topicKey];
        }

        public bool InitializeTopicDataService(string tenant, string product, string component, Topic topic)
        {
            string topicKey = ConnectorHelper.GetTopicKey(tenant, product, component, topic.Name);
            if (_topicDataServices.ContainsKey(topicKey))
                return false;

            var topicSettings = _coreRepository.GetTopicSettings(tenant, product, component, topic.Name);
            return _topicDataServices.TryAdd(topicKey, new TopicRocksDbDataService(
                _loggerFactory,
                tenant,
                product,
                component,
                topic.Name,
                _storageConfiguration,
                topicSettings,
                _tenantStateRepository,
                _coreRepository));
        }

        public bool InitializeTopicReadonlyDataService(string tenant, string product, string component, Topic topic)
        {
            string topicKey = ConnectorHelper.GetTopicKey(tenant, product, component, topic.Name);
            if (_topicReadonlyDataServices.ContainsKey(topicKey))
                return false;

            var topicSettings = _coreRepository.GetTopicSettings(tenant, product, component, topic.Name);
            return _topicReadonlyDataServices.TryAdd(topicKey, new TopicRocksDbReadonlyDataService(tenant, product, component, topic.Name, _storageConfiguration, topicSettings));
        }


        public bool InitializeSubscriptionUnackedDataService(string tenant, string product, string component, string topic, string subscription)
        {
            string subscriptionKey = ConnectorHelper.GetSubcriptionId(tenant, product, component, topic, subscription);
            if (_subscriptionUnackedDataServices.ContainsKey(subscriptionKey))
                return false;

            return _subscriptionUnackedDataServices.TryAdd(subscriptionKey, new SubscriptionUnackedDataService(tenant, product, component, topic, subscription, _storageConfiguration));
        }

        public bool InitializeSubscriptionUnackedReadonlyDataService(string tenant, string product, string component, string topic, string subscription)
        {
            string subscriptionKey = ConnectorHelper.GetSubcriptionId(tenant, product, component, topic, subscription);
            if (_subscriptionUnackedReadonlyDataServices.ContainsKey(subscriptionKey))
                return false;

            return _subscriptionUnackedReadonlyDataServices.TryAdd(subscriptionKey, new SubscriptionUnackedReadonlyDataService(tenant, product, component, topic, subscription, _storageConfiguration));
        }

        public ITopicDataService<UnacknowledgedMessage> GetSubscriptionUnackedDataService(string subscriptionKey)
        {
            if (_subscriptionUnackedDataServices.ContainsKey(subscriptionKey) != true)
                return null;

            return _subscriptionUnackedDataServices[subscriptionKey];
        }

        public ITopicReadonlyDataService<UnacknowledgedMessage> GetSubscriptionUnackedReadonlyDataService(string subscriptionKey)
        {
            if (_subscriptionUnackedReadonlyDataServices.ContainsKey(subscriptionKey) != true)
                return null;

            return _subscriptionUnackedReadonlyDataServices[subscriptionKey];
        }

        public ITopicDataService<ClusterChangeLog> GetClusterDataService(string nodeId)
        {
            if (_nodesDataServices.ContainsKey(nodeId) != true)
                return null;

            return _nodesDataServices[nodeId];
        }

        public void InitializeClusterDataService(Replica replica)
        {
            // Initialize NodeRocksDbService
            if (_nodesDataServices.ContainsKey(replica.NodeId) != true)
            {
                var clusterDataService = new ClusterRocksDbDataService(_loggerFactory, replica, _storageConfiguration);
                _nodesDataServices.TryAdd(replica.NodeId, clusterDataService);
            }
        }

        public ConcurrentDictionary<string, ITopicDataService<ClusterChangeLog>> GetClusterDataServices()
        {
            return _nodesDataServices;
        }
    }
}
