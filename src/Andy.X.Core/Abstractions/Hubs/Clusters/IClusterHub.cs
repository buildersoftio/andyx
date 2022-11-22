using Buildersoft.Andy.X.Model.Clusters.Events;
using Buildersoft.Andy.X.Model.Entities.Clusters;
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

        Task TenantRetentionCreatedAsync(TenantRetentionCreatedArgs tenantRetentionArgs);
        Task TenantRetentionUpdatedAsync(TenantRetentionUpdatedArgs tenantRetentionArgs);
        Task TenantRetentionDeletedAsync(TenantRetentionDeletedArgs tenantRetentionArgs);

        Task ProductCreatedAsync(ProductCreatedArgs productCreatedArgs);
        Task ProductUpdatedAsync(ProductUpdatedArgs productUpdatedArgs);
        Task ProductDeletedAsync(ProductDeletedArgs productDeletedArgs);

        Task ProductRetentionCreatedAsync(ProductRetentionCreatedArgs productRetentionArgs);
        Task ProductRetentionUpdatedAsync(ProductRetentionUpdatedArgs productRetentionArgs);
        Task ProductRetentionDeletedAsync(ProductRetentionDeletedArgs productRetentionArgs);

        Task ComponentCreatedAsync(ComponentCreatedArgs componentCreatedArgs);
        Task ComponentUpdatedAsync(ComponentUpdatedArgs componentUpdatedArgs);
        Task ComponentDeletedAsync(ComponentDeletedArgs componentDeletedArgs);

        Task ComponentRetentionCreatedAsync(ComponentRetentionCreatedArgs componentRetentionArgs);
        Task ComponentRetentionUpdatedAsync(ComponentRetentionUpdatedArgs componentRetentionArgs);
        Task ComponentRetentionDeletedAsync(ComponentRetentionDeletedArgs componentRetentionArgs);

        Task TopicCreatedAsync(TopicCreatedArgs topicCreatedArgs);
        Task TopicUpdatedAsync(TopicUpdatedArgs topicUpdatedArgs);
        Task TopicDeletedAsync(TopicDeletedArgs topicDeletedArgs);

        Task TenantTokenCreatedAsync(TenantTokenCreatedArgs tokenCreatedArgs);
        Task TenantTokenRevokedAsync(TenantTokenRevokedArgs tokenRevokedArgs);
        Task TenantTokenDeletedAsync(TenantTokenDeletedArgs tokenDeletedArgs);

        Task ProductTokenCreatedAsync(ProductTokenCreatedArgs productTokenCreatedArgs);
        Task ProductTokenRevokedAsync(ProductTokenRevokedArgs productTokenRevokedArgs);
        Task ProductTokenDeletedAsync(ProductTokenDeletedArgs productTokenDeletedArgs);

        Task ComponentTokenCreatedAsync(ComponentTokenCreatedArgs tokenCreatedArgs);
        Task ComponentTokenRevokedAsync(ComponentTokenRevokedArgs tokenRevokedArgs);
        Task ComponentTokenDeletedAsync(ComponentTokenDeletedArgs tokenDeletedArgs);

        Task SubscriptionCreatedAsync(SubscriptionCreatedArgs subscriptionCreatedArgs);
        Task SubscriptionUpdatedAsync(SubscriptionUpdatedArgs subscriptionUpdatedArgs);
        Task SubscriptionDeletedAsync(SubscriptionDeletedArgs subscriptionDeletedArgs);

        // will all nodes
        Task ProducerConnectedAsync(ProducerConnectedArgs producerConnectedArgs);
        Task ProducerCreatedAsync(ProducerCreatedArgs producerCreatedArgs);
        Task ProducerDeletedAsync(ProducerDeletedArgs producerDeletedArgs);
        Task ProducerDisconnectedAsync(ProducerDisconnectedArgs producerDisconnectedArgs);

        Task ConsumerConnectedAsync(ConsumerConnectedArgs consumerConnectedArgs);
        Task ConsumerDisconnectedAsync(ConsumerDisconnectedArgs consumerDisconnectedArgs);


        // only with replicas
        Task SubscriptionPositionUpdatedAsync(SubscriptionPositionUpdatedArgs subscriptionPositionUpdatedArgs);
        Task CurrentEntryPositionUpdatedAsync(CurrentEntryPositionUpdatedArgs currentEntryPositionUpdatedArgs);

        // Distributed Clusters, communication between main nodes
        Task SendMessageToMainShardAsync(ClusterChangeLog clusterChangeLog);
    }
}
