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

        public Consumer CreateConsumer(string tenant, string product, string component, string topic, string consumerName, SubscriptionType consumerType)
        {
            return new Consumer()
            {
                Id = Guid.NewGuid(),
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                ConsumerName = consumerName,
                SubscriptionType = consumerType
            };
        }
    }
}
