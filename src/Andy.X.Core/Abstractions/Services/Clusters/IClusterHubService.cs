using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Entities.Subscriptions;
using Buildersoft.Andy.X.Model.Subscriptions;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Clusters
{
    public interface IClusterHubService
    {
        // this service is designed to comunicate only with Cluster Syncs between each nodes. to share and synchronize cluster metadata like writen below.


        Task ClusterMetadataSynchronization_AllNodes(/*TODO: create a model that has data for synchronization between nodes*/);

        // to all nodes

        Task CreateTenant_AllNodes(string name, Model.Entities.Core.Tenants.TenantSettings tenantSettings);
        Task UpdateTenant_AllNodes(string name, Model.Entities.Core.Tenants.TenantSettings tenantSettings);
        Task DeleteTenant_AllNodes(string name);


        Task CreateProduct_AllNodes(string tenant, Product product);
        Task UpdateProduct_AllNodes(string tenant, Product product);
        Task DeleteProduct_AllNodes(string tenant, string product);


        Task CreateComponent_AllNodes(string tenant, string product, Component component);
        Task UpdateComponent_AllNodes(string tenant, string product, Component component);
        Task DeleteComponent_AllNodes(string tenant, string product, string component);

        Task CreateTopic_AllNodes(string tenant, string product, string component,Topic topic);
        Task UpdateTopic_AllNodes(string tenant, string product, string component,Topic topic);
        Task DeleteTopic_AllNodes(string tenant, string product, string component, string topic);

        Task CreateTenantToken_AllNodes(TenantToken tenantToken);
        Task RevokeTenantToken_AllNodes(string tenant, string key);

        Task CreateComponentToken_AllNodes(ComponentToken tenantToken);
        Task RevokeComponentToken_AllNodes(string tenant, string product, string component, string key);

        Task CreateSubscription_AllNodes(Subscription subscription);
        Task UpdateSubscription_AllNodes(Subscription subscription);
        Task DeleteSubscription_AllNodes(string tenant, string product, string component, string topic, string subscription);

        Task ConnectProducer_AllNodes(string tenant, string product, string component, string topic, string producerConnectionId, string producer);
        Task DisconnectProducer_AllNodes(string tenant, string product, string component, string topic, string producerConnectionId);

        Task ConnectConsumer_AllNodes(string tenant, string product, string component, string topic, Subscription subscription, string consumerConnectionId, string consumer);
        Task DisconnectConsumer_AllNodes(string tenant, string product, string component, string topic, string subscription, string consumerConnectionId);

        // only between replicas
        Task UpdateSubscriptionPosition_ToReplica(string tenant, string product, string component, string topic,string subscriptionName, SubscriptionPosition subscription);
        Task UpdateEntryPosition_ToReplica(string tenant, string product, string component, string topic, TopicStates topicStates);
    }
}
