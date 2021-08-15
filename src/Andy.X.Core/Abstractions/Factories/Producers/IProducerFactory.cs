using Buildersoft.Andy.X.Model.Producers;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Producers
{
    public interface IProducerFactory
    {
        Producer CreateProducer();
        Producer CreateProducer(string tenant, string product, string component, string topic, string producerName);
    }
}
