﻿using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Outbound
{
    public interface IOutboundMessageService
    {
        Task AddSubscriptionTopicData(SubscriptionTopicData subscriptionTopicData);

        Task SendAllMessages(string subscriptionId);
        bool CheckIfUnackedMessagesExists(string subscriptionId, long entryId);
        void DeleteEntryOfUnackedMessages(string subscriptionId);

        Task SendFirstMessage(string subscriptionId, long currentEntryId);

        // This method is needed for 'NonResilient' Mode Subscription
        Task UpdateCurrentPosition(string subscriptionId, long currentEntryId);

        Task SendNextMessage(string subscriptionId, long currentEntryId);
        Task SendSameMessage(string subscriptionId, long currentEntryId);

        Task StoreCurrentPositionAsync(string subscriptionId);
        Task StopOutboundMessageServiceForSubscription(string subscriptionId);
        Task StartOutboundMessageServiceForSubscription(string subscriptionId);

        Task TriggerSubscriptionsByProducer(string tenant, string product, string component, string topic);

        SubscriptionTopicData GetSubscriptionDataConnector(string subscriptionId);
    }
}
