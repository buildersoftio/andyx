using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.IO.Services;
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
            TenantIOService.TryCreateProducerDirectory(producer.Tenant, producer.Product, producer.Component, producer.Topic, producer.ProducerName);
            return _producers.TryAdd(connectionId, producer);
        }
        public List<string> GetAllProducers()
        {
            return _producers.Keys.ToList();
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

        public Dictionary<string, Producer> GetProducers(string tenant, string product, string component, string topic)
        {
            try
            {
                return _producers
                .Where(x => x.Value.Tenant == tenant &&
                       x.Value.Product == product &&
                       x.Value.Component == component &&
                       x.Value.Topic == topic)
                .ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception)
            {
                return new Dictionary<string, Producer>();
            }
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

        public List<ProducerActivity> GetAllProducerActivities()
        {
            var results = new List<ProducerActivity>();

            foreach (var sub in _producers)
            {
                results.Add(new ProducerActivity()
                {
                    Name = sub.Value.ProducerName,
                    Location = $"{sub.Value.Tenant}/{sub.Value.Product}/{sub.Value.Component}/{sub.Value.Topic}",
                    IsActive = true
                });
            }

            return results;
        }
    }
}
