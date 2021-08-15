using Buildersoft.Andy.X.Model.Consumers;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers
{
    public interface IConsumerFactory
    {
        Consumer CreateConsumer();
        Consumer CreateConsumer(string tenant, string product, string component, string topic, string consumerName, SubscriptionType consumerType);
    }
}
