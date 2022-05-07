using Buildersoft.Andy.X.Model.Producers;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Producers
{
    public interface IProducerHubRepository
    {
        bool AddProducer(string connectionId, Producer producer);
        bool RemoveProducer(string connectionId);

        Producer GetProducerById(string connectionId);
        KeyValuePair<string, Producer> GetProducerByProducerName(string tenant, string product, string component, string topic, string producerName);
        Dictionary<string, Producer> GetProducersByTenantName(string tenant);
        Dictionary<string, Producer> GetProducers(string tenant, string product, string component, string topic);

        List<string> GetAllProducers();
    }
}
