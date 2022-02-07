using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
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

        public bool AddConsumer(string consumerName, Consumer consumer)
        {
            return _consumers.TryAdd(consumerName, consumer);
        }

        public bool AddConsumerConnection(string consumerName, string connectionId)
        {
            if (_consumers.ContainsKey(consumerName))
            {
                _consumers[consumerName].Connections.Add(connectionId);
                return true;
            }

            return false;
        }

        public List<string> GetAllConsumerNames()
        {
            return _consumers.Keys.ToList();
        }

        public Consumer GetConsumerByConnectionId(string connectionId)
        {
            return _consumers.Values.Where(x => x.Connections.Contains(connectionId)).FirstOrDefault();
        }

        public Consumer GetConsumerByName(string consumerName)
        {
            if (_consumers.ContainsKey(consumerName))
                return _consumers[consumerName];

            return null;
        }

        public Dictionary<string, Consumer> GetConsumersByTopic(string tenant, string product, string component, string topic)
        {
            try
            {
                return _consumers.Where(x => x.Value.Tenant == tenant
                    && x.Value.Product == product
                    && x.Value.Component == component
                    && x.Value.Topic == topic)
            .ToDictionary(x => x.Key, x => x.Value);
            }

            catch (System.Exception)
            {
                // return Empty Dictionary, there is not consumer connected.
                return new Dictionary<string, Consumer>();
            }

        }

        public bool RemoveConsumer(string consumerName)
        {
            if (_consumers.ContainsKey(consumerName))
            {
                if (_consumers[consumerName].Connections.Count == 0)
                    return _consumers.TryRemove(consumerName, out _);

                return true;
            }

            return false;
        }

        public bool RemoveConsumerConnection(string consumerName, string connectionId)
        {
            if (_consumers.ContainsKey(consumerName))
            {
                _consumers[consumerName].Connections.Remove(connectionId);
                return true;
            }

            return false;
        }
    }
}
