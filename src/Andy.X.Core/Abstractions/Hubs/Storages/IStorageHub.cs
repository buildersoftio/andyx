using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Producers.Events;
using Buildersoft.Andy.X.Model.Storages.Events.Agents;
using Buildersoft.Andy.X.Model.Storages.Events.Components;
using Buildersoft.Andy.X.Model.Storages.Events.Messages;
using Buildersoft.Andy.X.Model.Storages.Events.Products;
using Buildersoft.Andy.X.Model.Storages.Events.Tenants;
using Buildersoft.Andy.X.Model.Storages.Events.Topics;
using Buildersoft.Andy.X.Model.Storages.Requests.Tenants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Hubs.Storages
{
    public interface IStorageHub
    {
        Task StorageConnected(AgentConnectedDetails connectionDetails);
        Task StorageDisconnected(AgentDisconnectedDetails disconnectionDetail);

        Task ProducerConnected(ProducerConnectedDetails connectedDetails);
        Task ProducerDisconnected(ProducerDisconnectedDetails disconnectedDetails);

        Task ConsumerConnected(ConsumerConnectedDetails connectedDetails);
        Task ConsumerDisconnected(ConsumerDisconnectedDetails disconnectedDetails);
        Task ConsumerUnacknowledgedMessagesRequested(ConsumerConnectedDetails consumerConnectedDetails);


        Task TenantCreated(TenantCreatedDetails tenant);
        Task TenantCreatedToOtherNodes(CreateTenantDetails tenant);
        Task TenantUpdated(TenantUpdatedDetails tenant);
        Task TenantDeleted(TenantDeletedDetails tenant);

        Task TenantTokenCreated(TenantTokenCreatedDetails tenantTokenCreated);
        Task TenantTokenRevoked(TenantTokenRevokedDetails tenantTokenRevoked);


        Task ProductCreated(ProductCreatedDetails product);
        Task ProductUpdated(ProductUpdatedDetails product);
        Task ProductDeleted(ProductDeletedDetails product);

        Task ComponentCreated(ComponentCreatedDetails component);
        Task ComponentUpdated(ComponentUpdatedDetails component);
        Task ComponentDeleted(ComponentDeletedDetails component);

        Task ComponentTokenCreated(ComponentTokenCreatedDetails componentTokenCreated);
        Task ComponentTokenRevoked(ComponentTokenRevokedDetails componentTokenRevoked);

        Task TopicCreated(TopicCreatedDetails topic);
        Task TopicUpdated(TopicUpdatedDetails topic);
        Task TopicDeleted(TopicDeletedDetails topic);

        Task MessageStored(MessageStoredDetails message);
        Task MessagesStored(List<MessageStoredDetails> messages);
        Task MessageAcknowledged(MessageAcknowledgedDetails message);
    }
}
