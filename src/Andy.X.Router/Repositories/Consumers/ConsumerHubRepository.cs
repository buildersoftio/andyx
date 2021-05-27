using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.Consumers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Buildersoft.Andy.X.Router.Repositories.Consumers
{
    public class ConsumerHubRepository : IConsumerHubRepository
    {
        private readonly ILogger<ConsumerHubRepository> _logger;
        private ConcurrentDictionary<string, Consumer> _consumers;
        public ConsumerHubRepository(ILogger<ConsumerHubRepository> logger)
        {
            _logger = logger;
            _consumers = new ConcurrentDictionary<string, Consumer>();
        }

        public bool AddConsumer(string connectionId, Consumer consumer)
        {
            return _consumers.TryAdd(connectionId, consumer);
        }

        public Consumer GetConsumerById(string connectionId)
        {
            if (_consumers.ContainsKey(connectionId))
                return _consumers[connectionId];

            return null;
        }

        public KeyValuePair<string, Consumer> GetConsumerByConsumerName(string tenant, string product, string component, string topic, string consumerName)
        {
            return _consumers.Where(x => x.Value.Tenant == tenant
                && x.Value.Product == product
                && x.Value.Component == component
                && x.Value.Topic == topic
                && x.Value.ConsumerName == consumerName).FirstOrDefault();
        }

        public Dictionary<string, Consumer> GetConsumersByTenantName(string tenantName)
        {
            return _consumers
              .Where(x => x.Value.Tenant == tenantName)
              .ToDictionary(x => x.Key, x => x.Value);
        }

        public bool RemoveConsumer(string connectionId)
        {
            return _consumers.TryRemove(connectionId, out _);
        }
    }
}
