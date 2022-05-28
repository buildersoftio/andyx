using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Outbound
{
    public interface IOutboundMessageService
    {
        Task AddSubscriptionTopicData(SubscriptionTopicData subscriptionTopicData);

        Task SendFirstMessage(string subscriptionId, long currentLedgerId, long currentEntryId);

        Task SendNextMessage(string subscriptionId, long currentLedgerId, long currentEntryId);
        Task SendSameMessage(string subscriptionId, long currentLedgerId, long currentEntryId);

        Task StoreCurrentPosition(string subscriptionId);
        Task StopOutboundMessageServiceForSubscription(string subscriptionId);
        Task StartOutboundMessageServiceForSubscription(string subscriptionId);
    }
}
