using Buildersoft.Andy.X.Core.Abstractions.Hubs.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.Core.Services.Data;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Clusters;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Subscriptions;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Router.Hubs.Clusters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Component = Buildersoft.Andy.X.Model.App.Components.Component;

namespace Buildersoft.Andy.X.Router.Services.Clusters
{
    public class ClusterHubService : IClusterHubService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ClusterHubService> _logger;
        private readonly IClusterHubRepository _clusterHubRepository;
        private readonly IHubContext<ClusterHub, IClusterHub> _hub;
        private readonly IClusterRepository _clusterRepository;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly StorageConfiguration _storageConfiguration;

        private readonly ConcurrentDictionary<string, ITopicDataService<ClusterChangeLog>> _nodesDataServices;

        public ClusterHubService(ILoggerFactory logger,
            IClusterHubRepository clusterHubRepository,
            IHubContext<ClusterHub, IClusterHub> hub,
            IClusterRepository clusterRepository,
            NodeConfiguration nodeConfiguration,
            StorageConfiguration storageConfiguration)
        {
            _loggerFactory = logger;
            _logger = logger.CreateLogger<ClusterHubService>();

            _clusterHubRepository = clusterHubRepository;
            _hub = hub;
            _clusterRepository = clusterRepository;
            _nodeConfiguration = nodeConfiguration;
            _storageConfiguration = storageConfiguration;

            _nodesDataServices = new ConcurrentDictionary<string, ITopicDataService<ClusterChangeLog>>();
        }

        public Task ClusterMetadataSynchronization_AllNodes()
        {
            // HERE WE GO. hmm let see how we do it, in case if we will add a new node into an existing cluster. :p
            throw new NotImplementedException();
        }

        public Task ConnectConsumer_AllNodes(string tenant, string product, string component, string topic, Subscription subscription, string consumerConnectionId, string consumer)
        {
            return _hub.Clients.All.ConsumerConnectedAsync(new Model.Clusters.Events.ConsumerConnectedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                Subscription = subscription.SubscriptionName,

                Consumer = consumer,
                ConsumerConnectionId = consumerConnectionId
            });
        }

        public Task ConnectProducer_AllNodes(string tenant, string product, string component, string topic, string producerConnectionId, string producer)
        {
            return _hub.Clients.All.ProducerConnectedAsync(new Model.Clusters.Events.ProducerConnectedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                Producer = producer,
                ProducerConnectionId = producerConnectionId
            });
        }

        public Task CreateComponentToken_AllNodes(ComponentToken tenantToken)
        {
            throw new NotImplementedException();
        }

        public Task CreateComponent_AllNodes(string tenant, string product, Component component)
        {
            return _hub.Clients.All.ComponentCreatedAsync(new Model.Clusters.Events.ComponentCreatedArgs()
            {
                Name = component.Name,
                Tenant = tenant,
                Product = product,
                Settings = component.Settings,
            });
        }

        public Task CreateProduct_AllNodes(string tenant, Product product)
        {
            return _hub.Clients.All.ProductCreatedAsync(new Model.Clusters.Events.ProductCreatedArgs()
            {
                Tenant = tenant,

                Description = product.Description,
                Name = product.Name
            });
        }

        public Task CreateSubscription_AllNodes(Subscription subscription)
        {
            return _hub.Clients.All.SubscriptionCreatedAsync(new Model.Clusters.Events.SubscriptionCreatedArgs()
            {
                Tenant = subscription.Tenant,
                Product = subscription.Product,
                Component = subscription.Component,
                Topic = subscription.Topic,
                SubscriptionName = subscription.SubscriptionName,
                InitialPosition = subscription.InitialPosition,
                SubscriptionMode = subscription.SubscriptionMode,
                SubscriptionType = subscription.SubscriptionType
            });
        }

        public Task CreateTenantToken_AllNodes(TenantToken tenantToken)
        {
            throw new NotImplementedException();
        }

        public Task CreateTenant_AllNodes(string name, Model.Entities.Core.Tenants.TenantSettings tenantSettings)
        {
            return _hub.Clients.All.TenantCreatedAsync(new Model.Clusters.Events.TenantCreatedArgs()
            {
                Name = name,
                Settings = tenantSettings
            });
        }

        public Task CreateTopic_AllNodes(string tenant, string product, string component, Topic topic)
        {
            return _hub.Clients.All.TopicCreatedAsync(new Model.Clusters.Events.TopicCreatedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,

                Name = topic.Name,
                TopicStates = topic.TopicStates
            });
        }

        public Task DeleteComponent_AllNodes(string tenant, string product, string component)
        {
            return _hub.Clients.All.ComponentDeletedAsync(new Model.Clusters.Events.ComponentDeletedArgs()
            {
                Tenant = tenant,
                Product = product,

                Name = component
            });
        }

        public Task DeleteProduct_AllNodes(string tenant, string product)
        {
            return _hub.Clients.All.ProductDeletedAsync(new Model.Clusters.Events.ProductDeletedArgs()
            {
                Tenant = tenant,
                Name = product
            });
        }

        public Task DeleteSubscription_AllNodes(string tenant, string product, string component, string topic, string subscription)
        {
            return _hub.Clients.All.SubscriptionDeletedAsync(new Model.Clusters.Events.SubscriptionDeletedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,

                Topic = topic,
                SubscriptionName = subscription
            });
        }

        public Task DeleteTenant_AllNodes(string name)
        {
            return _hub.Clients.All.TenantDeletedAsync(new Model.Clusters.Events.TenantDeletedArgs()
            {
                Name = name
            });
        }

        public Task DeleteTopic_AllNodes(string tenant, string product, string component, string topic)
        {
            return _hub.Clients.All.TopicDeletedAsync(new Model.Clusters.Events.TopicDeletedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,

                Name = topic
            });
        }

        public Task DisconnectConsumer_AllNodes(string tenant, string product, string component, string topic, string subscription, string consumerConnectionId)
        {
            return _hub.Clients.All.ConsumerDisconnectedAsync(new Model.Clusters.Events.ConsumerDisconnectedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,

                Subscription = subscription,
                ConsumerConnectionId = consumerConnectionId,
            });
        }

        public Task DisconnectProducer_AllNodes(string tenant, string product, string component, string topic, string producerConnectionId)
        {
            return _hub.Clients.All.ProducerDisconnectedAsync(new Model.Clusters.Events.ProducerDisconnectedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                ProducerConnectionId = producerConnectionId
            });
        }

        public Task RevokeComponentToken_AllNodes(string tenant, string product, string component, string key)
        {
            throw new NotImplementedException();
        }

        public Task RevokeTenantToken_AllNodes(string tenant, string key)
        {
            throw new NotImplementedException();
        }

        public Task UpdateComponent_AllNodes(string tenant, string product, Component component)
        {
            return _hub.Clients.All.ComponentUpdatedAsync(new Model.Clusters.Events.ComponentUpdatedArgs()
            {
                Tenant = tenant,
                Product = product,
                Settings = component.Settings,
                Name = component.Name
            });
        }

        public Task UpdateSubscription_AllNodes(Subscription subscription)
        {
            return _hub.Clients.All.SubscriptionUpdatedAsync(new Model.Clusters.Events.SubscriptionUpdatedArgs()
            {
                Tenant = subscription.Tenant,
                Product = subscription.Product,
                Component = subscription.Component,
                Topic = subscription.Topic,
                InitialPosition = subscription.InitialPosition,
                SubscriptionMode = subscription.SubscriptionMode,
                SubscriptionName = subscription.SubscriptionName,
                SubscriptionType = subscription.SubscriptionType
            });
        }

        public Task UpdateTenant_AllNodes(string name, TenantSettings tenantSettings)
        {
            return _hub.Clients.All.TenantUpdatedAsync(new Model.Clusters.Events.TenantUpdatedArgs()
            {
                Name = name,
                Settings = tenantSettings
            });
        }

        public Task UpdateTopic_AllNodes(string tenant, string product, string component, Topic topic)
        {
            return _hub.Clients.All.TopicUpdatedAsync(new Model.Clusters.Events.TopicUpdatedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Name = topic.Name
            });
        }

        public Task UpdateProduct_AllNodes(string tenant, Product product)
        {
            return _hub.Clients.All.ProductUpdatedAsync(new Model.Clusters.Events.ProductUpdatedArgs()
            {
                Tenant = tenant,
                Name = product.Name,

                Description = product.Description,
            });
        }

        public async Task UpdateEntryPosition_ToReplica(string tenant, string product, string component, string topic, TopicStates topicStates)
        {
            // update entryposition from main replica to other replicas.
            var currentShard = _clusterRepository.GetCurrentShard();
            foreach (var replica in currentShard.Replicas)
            {
                if (replica.IsLocal == true && replica.Type == Model.Clusters.ReplicaTypes.Main)
                    continue;

                var replicaConnectionId = _clusterHubRepository.GetNodeConnectionIdByNodeId(replica.NodeId);

                await _hub.Clients.Client(replicaConnectionId).CurrentEntryPositionUpdatedAsync(new Model.Clusters.Events.CurrentEntryPositionUpdatedArgs()
                {
                    Component = component,
                    Product = product,
                    Tenant = tenant,
                    Topic = topic,
                    TopicStates = topicStates
                });
            }
        }

        public async Task UpdateSubscriptionPosition_ToReplica(string tenant, string product, string component, string topic, string subscriptionName, SubscriptionPosition subscription)
        {
            // update subscription position from main replica to other replicas.
            var currentShard = _clusterRepository.GetCurrentShard();
            foreach (var replica in currentShard.Replicas)
            {
                if (replica.IsLocal == true && replica.Type == Model.Clusters.ReplicaTypes.Main)
                    continue;

                var replicaConnectionId = _clusterHubRepository.GetNodeConnectionIdByNodeId(replica.NodeId);
                await _hub.Clients.Client(replicaConnectionId).SubscriptionPositionUpdatedAsync(new Model.Clusters.Events.SubscriptionPositionUpdatedArgs()
                {
                    Tenant = tenant,
                    Product = product,
                    Component = component,
                    Topic = topic,
                    SubscriptionName = subscriptionName,

                    SubscriptionPosition = subscription
                });
            }
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
    }
}
