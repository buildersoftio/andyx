using Buildersoft.Andy.X.Model.Consumers;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers
{
    public interface IConsumerHubRepository
    {
        bool AddConsumer(string connectionId, Consumer consumer);
        bool RemoveConsumer(string connectionId);

        Consumer GetConsumerById(string connectionId);
        KeyValuePair<string, Consumer> GetConsumerByConsumerName(string tenant, string product, string component, string topic, string consumerName);
        Dictionary<string, Consumer> GetConsumersByTenantName(string tenantName);
    }
}
