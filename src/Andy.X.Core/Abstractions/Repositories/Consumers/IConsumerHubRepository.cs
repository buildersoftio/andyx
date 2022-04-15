using Buildersoft.Andy.X.Model.Consumers;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers
{
    public interface IConsumerHubRepository
    {
        bool AddConsumer(string consumerId, Consumer consumer);
        bool AddConsumerConnection(string consumerId, string connectionId);
        bool AddExternalConsumerConnection(string consumerId);
        bool RemoveConsumer(string consumerId);
        bool RemoveConsumerConnection(string consumerId, string connectionId);
        bool RemoveExternalConsumerConnection(string consumerId);

        Consumer GetConsumerById(string consumerName);
        Consumer GetConsumerByConnectionId(string connectionId);

        Dictionary<string, Consumer> GetConsumersByTopic(string tenant, string product, string component, string topic);

        List<string> GetAllConsumerNames();
    }
}
