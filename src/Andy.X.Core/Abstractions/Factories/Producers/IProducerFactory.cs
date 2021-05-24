using Buildersoft.Andy.X.Model.Producers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Producers
{
    public interface IProducerFactory
    {
        Producer CreateProducer();
        Producer CreateProducer(string tenant, string product, string component, string topic, string producerName);
    }
}
