using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Model.Producers;
using System;

namespace Buildersoft.Andy.X.Core.Factories.Producers
{
    public class ProducerFactory : IProducerFactory
    {
        public Producer CreateProducer()
        {
            return new Producer();
        }

        public Producer CreateProducer(string tenant, string product, string component, string topic, string producerName)
        {
            return new Producer()
            {
                Id = Guid.NewGuid(),
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                ProducerName = producerName
            };
        }
    }
}
