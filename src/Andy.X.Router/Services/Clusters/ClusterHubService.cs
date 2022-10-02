using Buildersoft.Andy.X.Core.Abstractions.Hubs.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Clusters;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using Buildersoft.Andy.X.Model.Entities.Subscriptions;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Router.Hubs.Clusters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
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

        public Task CreateComponentToken_AllNodes(string tenant, string product, string component, ComponentToken componentToken)
        {
            return _hub.Clients.All.ComponentTokenCreatedAsync(new Model.Clusters.Events.ComponentTokenCreatedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                ComponentToken = componentToken
            });
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

        public Task CreateProduct_AllNodes(string tenant, Model.App.Products.Product product)
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

        public Task CreateTenantToken_AllNodes(string tenant, TenantToken tenantToken)
        {
            return _hub.Clients.All.TenantTokenCreatedAsync(new Model.Clusters.Events.TenantTokenCreatedArgs()
            {
                Tenant = tenant,
                TenantToken = tenantToken
            });
        }

        public Task CreateTenant_AllNodes(string name, TenantSettings tenantSettings)
        {
            return _hub.Clients.All.TenantCreatedAsync(new Model.Clusters.Events.TenantCreatedArgs()
            {
                Name = name,
                Settings = tenantSettings
            });
        }

        public Task CreateTopic_AllNodes(string tenant, string product, string component, Model.App.Topics.Topic topic)
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

        public Task RevokeComponentToken_AllNodes(string tenant, string product, string component, Guid key)
        {
            return _hub.Clients.All.ComponentTokenRevokedAsync(new Model.Clusters.Events.ComponentTokenRevokedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Key = key
            });
        }

        public Task RevokeTenantToken_AllNodes(string tenant, Guid key)
        {
            return _hub.Clients.All.TenantTokenRevokedAsync(new Model.Clusters.Events.TenantTokenRevokedArgs()
            {
                Tenant = tenant,
                Key = key
            });
        }

        public Task UpdateComponent_AllNodes(string tenant, string product, string component, ComponentSettings componentSettings)
        {
            return _hub.Clients.All.ComponentUpdatedAsync(new Model.Clusters.Events.ComponentUpdatedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Settings = componentSettings
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

        public Task UpdateTopic_AllNodes(string tenant, string product, string component, string topic, TopicSettings topicSettings)
        {
            return _hub.Clients.All.TopicUpdatedAsync(new Model.Clusters.Events.TopicUpdatedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Name = topic,
                TopicSettings = topicSettings
            });
        }

        public Task UpdateProduct_AllNodes(string tenant, string product, ProductSettings productSettings)
        {
            return _hub.Clients.All.ProductUpdatedAsync(new Model.Clusters.Events.ProductUpdatedArgs()
            {
                Tenant = tenant,
                Name = product,
                ProductSettings = productSettings
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
                if (replica.IsLocal == true && replica.Type == ReplicaTypes.Main)
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

        public async Task DistributeMessage_ToNode(string nodeId, string connectionId, ClusterChangeLog clusterChangeLog)
        {
            //var connectionId = _clusterHubRepository.GetNodeConnectionIdByNodeId(nodeId);
            await _hub
                .Clients
                .Client(connectionId)
                .SendMessageToMainShardAsync(clusterChangeLog);
        }

        public Task CreateProductToken_AllNodes(string tenant, string product, Model.Entities.Core.Products.ProductToken productToken)
        {
            return _hub.Clients.All.ProductTokenCreatedAsync(new Model.Clusters.Events.ProductTokenCreatedArgs()
            {
                Tenant = tenant,
                Product = product,
                ProductToken = productToken
            });
        }

        public Task RevokeProductToken_AllNodes(string tenant, string product, Guid key)
        {
            return _hub.Clients.All.ProductTokenRevokedAsync(new Model.Clusters.Events.ProductTokenRevokedArgs()
            {
                Tenant = tenant,
                Product = product,
                Key = key
            });
        }

        public Task CreateTenantRetention_AllNodes(string tenant, TenantRetention tenantRetention)
        {
            return _hub.Clients.All.TenantRetentionCreatedAsync(new Model.Clusters.Events.TenantRetentionCreatedArgs()
            {
                Tenant = tenant,
                TenantRetention = tenantRetention
            });
        }

        public Task UpdateTenantRetention_AllNodes(string tenant, TenantRetention tenantRetention)
        {
            return _hub.Clients.All.TenantRetentionUpdatedAsync(new Model.Clusters.Events.TenantRetentionUpdatedArgs()
            {
                Tenant = tenant,
                TenantRetention = tenantRetention
            });
        }

        public Task DeleteTenantRetention_AllNodes(string tenant, TenantRetention tenantRetention)
        {
            return _hub.Clients.All.TenantRetentionDeletedAsync(new Model.Clusters.Events.TenantRetentionDeletedArgs()
            {
                Tenant = tenant,
                TenantRetention = tenantRetention
            });
        }

        public Task CreateProductRetention_AllNodes(string tenant, string product, Model.Entities.Core.Products.ProductRetention productRetention)
        {
            return _hub.Clients.All.ProductRetentionCreatedAsync(new Model.Clusters.Events.ProductRetentionCreatedArgs()
            {
                Tenant = tenant,
                Product = product,
                ProductRetention = productRetention
            });
        }

        public Task UpdateProductRetention_AllNodes(string tenant, string product, Model.Entities.Core.Products.ProductRetention productRetention)
        {
            return _hub.Clients.All.ProductRetentionUpdatedAsync(new Model.Clusters.Events.ProductRetentionUpdatedArgs()
            {
                Tenant = tenant,
                Product = product,
                ProductRetention = productRetention
            });
        }

        public Task DeleteProductRetention_AllNodes(string tenant, string product, Model.Entities.Core.Products.ProductRetention productRetention)
        {
            return _hub.Clients.All.ProductRetentionDeletedAsync(new Model.Clusters.Events.ProductRetentionDeletedArgs()
            {
                Tenant = tenant,
                Product = product,
                ProductRetention = productRetention
            });
        }

        public Task CreateComponentRetention_AllNodes(string tenant, string product, string component, ComponentRetention componentRetention)
        {
            return _hub.Clients.All.ComponentRetentionCreatedAsync(new Model.Clusters.Events.ComponentRetentionCreatedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                ComponentRetention = componentRetention
            });
        }

        public Task UpdateComponentRetention_AllNodes(string tenant, string product, string component, ComponentRetention componentRetention)
        {
            return _hub.Clients.All.ComponentRetentionUpdatedAsync(new Model.Clusters.Events.ComponentRetentionUpdatedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                ComponentRetention = componentRetention
            });
        }

        public Task DeleteComponentRetention_AllNodes(string tenant, string product, string component, ComponentRetention componentRetention)
        {
            return _hub.Clients.All.ComponentRetentionDeletedAsync(new Model.Clusters.Events.ComponentRetentionDeletedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                ComponentRetention = componentRetention
            });
        }

        public Task CreateProducer_AllNodes(string tenant, string product, string component, string topic, Model.Entities.Core.Producers.Producer producer)
        {
            return _hub.Clients.All.ProducerCreatedAsync(new Model.Clusters.Events.ProducerCreatedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                Producer = producer
            });
        }

        public Task DeleteProducer_AllNodes(string tenant, string product, string component, string topic, string producerName)
        {
            return _hub.Clients.All.ProducerDeletedAsync(new Model.Clusters.Events.ProducerDeletedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                Producer = producerName
            });
        }

        public Task DeleteTenantToken_AllNodes(string tenant, Guid key)
        {
            return _hub.Clients.All.TenantTokenDeletedAsync(new Model.Clusters.Events.TenantTokenDeletedArgs()
            {
                Tenant = tenant,
                Key = key
            });
        }

        public Task DeleteProductToken_AllNodes(string tenant, string product, Guid key)
        {
            return _hub.Clients.All.ProductTokenDeletedAsync(new Model.Clusters.Events.ProductTokenDeletedArgs()
            {
                Tenant = tenant,
                Product = product,
                Key = key
            });
        }

        public Task DeleteComponentToken_AllNodes(string tenant, string product, string component, Guid key)
        {
            return _hub.Clients.All.ComponentTokenDeletedAsync(new Model.Clusters.Events.ComponentTokenDeletedArgs()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Key = key
            });
        }
    }
}
