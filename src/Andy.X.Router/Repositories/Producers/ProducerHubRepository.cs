using Buildersoft.Andy.X.Core.Abstractions.Repositories.Producers;
using Buildersoft.Andy.X.Model.Producers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Buildersoft.Andy.X.Router.Repositories.Producers
{
    public class ProducerHubRepository : IProducerHubRepository
    {
        private readonly ILogger<ProducerHubRepository> _logger;
        private ConcurrentDictionary<string, Producer> _producers;

        public ProducerHubRepository(ILogger<ProducerHubRepository> logger)
        {
            _logger = logger;
            _producers = new ConcurrentDictionary<string, Producer>();
        }

        public bool AddProducer(string connectionId, Producer producer)
        {
            return _producers.TryAdd(connectionId, producer);
        }

        public Producer GetProducerById(string connectionId)
        {
            if (_producers.ContainsKey(connectionId))
                return _producers[connectionId];

            return null;
        }

        public KeyValuePair<string, Producer> GetProducerByProducerName(string tenant, string product, string component, string topic, string producerName)
        {
                return _producers.Where(x => x.Value.Tenant == tenant
                && x.Value.Product == product
                && x.Value.Component == component
                && x.Value.Topic == topic
                && x.Value.ProducerName == producerName).FirstOrDefault();
        }

        public Dictionary<string, Producer> GetProducersByTenantName(string tenant)
        {
            return _producers
                .Where(x => x.Value.Tenant == tenant)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public bool RemoveProducer(string connectionId)
        {
            return _producers.TryRemove(connectionId, out _);
        }
    }
}
