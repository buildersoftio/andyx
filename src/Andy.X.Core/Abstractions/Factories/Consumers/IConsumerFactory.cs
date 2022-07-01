using Buildersoft.Andy.X.Model.Consumers;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers
{
    public interface IConsumerFactory
    {
        Consumer CreateConsumer();
        Consumer CreateConsumer(string subscriptionName, string consumerName);
    }
}
