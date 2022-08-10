using Buildersoft.Andy.X.Model.Clusters.Events;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Producers.Events;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Hubs.Clusters
{
    public interface IClusterHub
    {
        // will all nodes, all nodes will notify the client node that is connected.
        Task NodeConnectedAsync(NodeConnectedArgs nodeConnectedArgs);
        Task NodeDisconnectedAsync(NodeDisconnectedArgs nodeDisconnectedArgs);

        // will all nodes
        Task ClusterSynchronizationAsync();

        // with all nodes
        Task TenantCreatedAsync(TenantCreatedArgs tenantCreatedArgs);
        Task TenantUpdatedAsync(TenantUpdatedArgs tenantUpdatedArgs);
        Task TenantDeletedAsync(TenantDeletedArgs tenantDeletedArgs);

        Task ProductCreatedAsync(ProductCreatedArgs productCreatedArgs);
        Task ProductUpdatedAsync(ProductUpdatedArgs productUpdatedArgs);
        Task ProductDeletedAsync(ProductDeletedArgs productDeletedArgs);

        Task ComponentCreatedAsync(ComponentCreatedArgs componentCreatedArgs);
        Task ComponentUpdatedAsync(ComponentUpdatedArgs componentUpdatedArgs);
        Task ComponentDeletedAsync(ComponentDeletedArgs componentDeletedArgs);

        Task TopicCreatedAsync(TopicCreatedArgs topicCreatedArgs);
        Task TopicUpdatedAsync(TopicUpdatedArgs topicUpdatedArgs);
        Task TopicDeletedAsync(TopicDeletedArgs topicDeletedArgs);

        Task TenantTokenCreatedAsync(TokenCreatedArgs tokenCreatedArgs);
        Task TenantTokenRevokedAsync(TokenRevokedArgs tokenRevokedArgs);
        Task ComponentTokenCreatedAsync(TokenCreatedArgs tokenCreatedArgs);
        Task ComponentTokenRevokedAsync(TokenRevokedArgs tokenRevokedArgs);

        Task SubscriptionCreatedAsync(SubscriptionCreatedArgs subscriptionCreatedArgs);
        Task SubscriptionUpdatedAsync(SubscriptionUpdatedArgs subscriptionUpdatedArgs);
        Task SubscriptionDeletedAsync(SubscriptionDeletedArgs subscriptionDeletedArgs);

        // will all nodes
        Task ProducerConnectedAsync(ProducerConnectedArgs producerConnectedArgs);
        Task ProducerDisconnectedAsync(ProducerDisconnectedArgs producerDisconnectedArgs);

        Task ConsumerConnectedAsync(ConsumerConnectedArgs consumerConnectedArgs);
        Task ConsumerDisconnectedAsync(ConsumerDisconnectedArgs consumerDisconnectedArgs);


        // only with replicas
        Task SubscriptionPositionUpdatedAsync(SubscriptionPositionUpdatedArgs subscriptionPositionUpdatedArgs);
        Task CurrentEntryPositionUpdatedAsync(CurrentEntryPositionUpdatedArgs currentEntryPositionUpdatedArgs);


    }
}
