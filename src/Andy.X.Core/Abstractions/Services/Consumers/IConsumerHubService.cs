using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Storages.Requests.Components;
using Buildersoft.Andy.X.Model.Storages.Requests.Consumer;
using Buildersoft.Andy.X.Model.Storages.Requests.Tenants;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Consumers
{
    public interface IConsumerHubService
    {
        Task TransmitMessage(Message message, bool isStoredAlready = false);
        Task TransmitMessageToConsumer(ConsumerMessage consumerMessage);

        Task CreateTenantTokenToThisNode(CreateTenantTokenDetails createTenantTokenDetails);
        Task RevokeTenantTokenToThisNode(RevokeTenantTokenDetails revokeTenantTokenDetails);

        Task CreateComponentTokenToThisNode(CreateComponentTokenDetails createComponentTokenDetails);
        Task RevokeComponentTokenToThisNode(RevokeComponentTokenDetails revokeComponentTokenDetails);
        Task CreateTenantToThisNode(CreateTenantDetails createTenantDetails);


        Task ConnectConsumerFromOtherNode(NotifyConsumerConnectionDetails notifyConsumerConnectionDetails);
        Task DisconnectConsumerFromOtherNode(NotifyConsumerConnectionDetails notifyConsumerConnectionDetails);
    }
}
