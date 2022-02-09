using Buildersoft.Andy.X.Model.Consumers;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers
{
    public interface IConsumerHubRepository
    {
        bool AddConsumer(string consumerName, Consumer consumer);
        bool AddConsumerConnection(string consumerName, string connectionId);
        bool RemoveConsumer(string consumerName);
        bool RemoveConsumerConnection(string consumerName, string connectionId);

        Consumer GetConsumerById(string consumerName);
        Consumer GetConsumerByConnectionId(string connectionId);
        Dictionary<string, Consumer> GetConsumersByTopic(string tenant, string product, string component, string topic);

        List<string> GetAllConsumerNames();
    }
}
