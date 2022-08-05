using Buildersoft.Andy.X.Model.Clusters.Events;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Producers.Events;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Hubs.Clusters
{
    public interface IClusterHub
    {
        // will all nodes
        Task NodeConnectedAsync();
        Task NodeDisconnectedAsync();

        // will all nodes
        Task ClusterSynchronizationAsync();

        // with all nodes
        Task TenantCreated(TenantCreatedArgs tenantCreatedArgs);
        Task TenantUpdated(TenantUpdatedArgs tenantUpdatedArgs);
        Task TenantDeleted(TenantDeletedArgs tenantDeletedArgs);

        Task ProductCreated(ProductCreatedArgs productCreatedArgs);
        Task ProductUpdated(ProductUpdatedArgs productUpdatedArgs);
        Task ProductDeleted(ProductDeletedArgs productDeletedArgs);

        Task ComponentCreated(ComponentCreatedArgs componentCreatedArgs);
        Task ComponentUpdated(ComponentUpdatedArgs componentUpdatedArgs);
        Task ComponentDeleted(ComponentDeletedArgs componentDeletedArgs);

        Task TopicCreated(TopicCreatedArgs topicCreatedArgs);
        Task TopicUpdated(TopicUpdatedArgs topicUpdatedArgs);
        Task TopicDeleted(TopicDeletedArgs topicDeletedArgs);

        Task TokenCreated(TokenCreatedArgs tokenCreatedArgs);
        Task TokenRevoked(TokenRevokedArgs tokenRevokedArgs);

        Task SubscriptionCreated(SubscriptionCreatedArgs subscriptionCreatedArgs);
        Task SubscriptionUpdated(SubscriptionUpdatedArgs subscriptionUpdatedArgs);
        Task SubscriptionDeleted(SubscriptionDeletedArgs subscriptionDeletedArgs);

        // only with replicas
        Task SubscriptionPositionUpdated(SubscriptionPositionUpdatedArgs subscriptionPositionUpdatedArgs);
        Task CurrentEntryPositionUpdated(CurrentEntryPositionUpdatedArgs currentEntryPositionUpdatedArgs);

        // will all nodes
        Task ProducerConnected(ProducerConnectedDetails producerConnectedDetails);
        Task ProducerDisconnected(ProducerDisconnectedDetails producerDisconnectedDetails);

        Task ConsumerConnected(ConsumerConnectedDetails consumerConnectedDetails);
        Task ConsumerDisconnected(ConsumerDisconnectedDetails consumerDisconnectedDetails);
    }
}
