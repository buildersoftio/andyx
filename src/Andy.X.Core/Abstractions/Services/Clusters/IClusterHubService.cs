using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Entities.Clusters;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Producers;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using Buildersoft.Andy.X.Model.Entities.Subscriptions;
using Buildersoft.Andy.X.Model.Subscriptions;
using System;
using System.Threading.Tasks;
using Component = Buildersoft.Andy.X.Model.App.Components.Component;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Clusters
{
    public interface IClusterHubService
    {
        // this service is designed to comunicate only with Cluster Syncs between each nodes. to share and synchronize cluster metadata like writen below.

        Task ClusterMetadataSynchronization_AllNodes(/*TODO: create a model that has data for synchronization between nodes*/);

        // to all nodes

        Task CreateTenant_AllNodes(string name, TenantSettings tenantSettings);
        Task UpdateTenant_AllNodes(string name, TenantSettings tenantSettings);
        Task DeleteTenant_AllNodes(string name);

        Task CreateTenantRetention_AllNodes(string tenant, TenantRetention tenantRetention);
        Task UpdateTenantRetention_AllNodes(string tenant, TenantRetention tenantRetention);
        Task DeleteTenantRetention_AllNodes(string tenant, TenantRetention tenantRetention);


        Task CreateProduct_AllNodes(string tenant, Product product, ProductSettings productSettings);
        Task UpdateProduct_AllNodes(string tenant, string product, ProductSettings productSettings);
        Task DeleteProduct_AllNodes(string tenant, string product);

        Task CreateProductRetention_AllNodes(string tenant, string product, ProductRetention productRetention);
        Task UpdateProductRetention_AllNodes(string tenant, string product, ProductRetention productRetention);
        Task DeleteProductRetention_AllNodes(string tenant, string product, ProductRetention productRetention);


        Task CreateComponent_AllNodes(string tenant, string product, Model.Entities.Core.Components.Component component, ComponentSettings componentSettings);
        Task UpdateComponent_AllNodes(string tenant, string product, string component, ComponentSettings componentSettings);
        Task DeleteComponent_AllNodes(string tenant, string product, string component);

        Task CreateComponentRetention_AllNodes(string tenant, string product, string component, ComponentRetention componentRetention);
        Task UpdateComponentRetention_AllNodes(string tenant, string product, string component, ComponentRetention componentRetention);
        Task DeleteComponentRetention_AllNodes(string tenant, string product, string component, ComponentRetention componentRetention);

        Task CreateTopic_AllNodes(string tenant, string product, string component, Model.Entities.Core.Topics.Topic topic, TopicSettings topicSettings);
        Task UpdateTopic_AllNodes(string tenant, string product, string component, string topic, TopicSettings topicSettings);
        Task DeleteTopic_AllNodes(string tenant, string product, string component, string topic);


        Task CreateTenantToken_AllNodes(string tenant, TenantToken tenantToken);
        Task RevokeTenantToken_AllNodes(string tenant, Guid key);
        Task DeleteTenantToken_AllNodes(string tenant, Guid key);

        Task CreateProductToken_AllNodes(string tenant, string product, ProductToken productToken);
        Task RevokeProductToken_AllNodes(string tenant, string product, Guid key);
        Task DeleteProductToken_AllNodes(string tenant, string product, Guid key);

        Task CreateComponentToken_AllNodes(string tenant, string product, string component, ComponentToken componentToken);
        Task RevokeComponentToken_AllNodes(string tenant, string product, string component, Guid key);
        Task DeleteComponentToken_AllNodes(string tenant, string product, string component, Guid key);


        Task CreateSubscription_AllNodes(Subscription subscription);
        Task UpdateSubscription_AllNodes(Subscription subscription);
        Task DeleteSubscription_AllNodes(string tenant, string product, string component, string topic, string subscription);

        Task ConnectProducer_AllNodes(string tenant, string product, string component, string topic, string producerConnectionId, string producer);
        Task CreateProducer_AllNodes(string tenant, string product, string component, string topic, Producer producer);
        Task DeleteProducer_AllNodes(string tenant, string product, string component, string topic, string producerName);
        Task DisconnectProducer_AllNodes(string tenant, string product, string component, string topic, string producerConnectionId);

        Task ConnectConsumer_AllNodes(string tenant, string product, string component, string topic, Subscription subscription, string consumerConnectionId, string consumer);
        Task DisconnectConsumer_AllNodes(string tenant, string product, string component, string topic, string subscription, string consumerConnectionId);



        // only between replicas
        Task UpdateSubscriptionPosition_ToReplica(string tenant, string product, string component, string topic, string subscriptionName, SubscriptionPosition subscription);
        Task UpdateEntryPosition_ToReplica(string tenant, string product, string component, string topic, TopicStates topicStates);



        // distributed cluster, messaging
        Task DistributeMessage_ToNode(string nodeId, string connectionId, ClusterChangeLog clusterChangeLog);
    }
}
