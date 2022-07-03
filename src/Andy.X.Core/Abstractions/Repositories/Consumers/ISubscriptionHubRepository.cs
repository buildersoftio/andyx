using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Subscriptions;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers
{
    public interface ISubscriptionHubRepository
    {

        bool AddSubscription(string subscriptionId, Topic topic, Subscription subscription);

        bool AddConsumer(string subscriptionId, string connectionId, Consumer consumer);
        bool AddExternalConsumer(string subscriptionId, string connectionId, Consumer consumer);

        bool RemoveConsumerConnection(string subscriptionId, string connectionId);
        bool RemoveExternalConsumerConnection(string subscriptionId, string externalConenctionId);

        Subscription GetSubscriptionById(string subscriptionId);
        Subscription GetSubscriptionByConnectionId(string connectionId);
        ConcurrentDictionary<string, Consumer> GetConsumersBySubscrptionId(string subscriptionId);
        Consumer GetConsumerByConnectionId(string connectionId);

        Dictionary<string, Subscription> GetSubscriptionsByTopic(string tenant, string product, string component, string topic);

        List<string> GetAllSubscriptionNames();
    }
}
