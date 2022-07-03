using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Subscriptions;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions
{
    public interface ISubscriptionFactory
    {
        Subscription CreateSubscription(string tenant, string product, string component, string topic, string subscriptionName, SubscriptionType subscriptionType, SubscriptionMode subscriptionMode, InitialPosition initialPosition);

        SubscriptionTopicData CreateSubscriptionTopicData(Subscription subscription, int flushCurrentPositionTimer, int backgroundIntervalReadMessages);

        MessageAcknowledgementFileContent CreateUnackAcknowledgedMessageContent(string tenant, string product, string component, string topic, string subscription, MessageAcknowledgedDetails message);
    }
}
