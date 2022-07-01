using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Model.Consumers;
using System;

namespace Buildersoft.Andy.X.Core.Factories.Consumers
{
    public class ConsumerFactory : IConsumerFactory
    {
        public Consumer CreateConsumer()
        {
            return new Consumer();
        }

        public Consumer CreateConsumer(string subscriptionName, string consumerName)
        {
            return new Consumer()
            {
                Id = Guid.NewGuid(),
                Name = consumerName,
                Subscription = subscriptionName,
            };
        }
    }
}
