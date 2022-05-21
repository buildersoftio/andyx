using Buildersoft.Andy.X.Core.Abstractions.Repositories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Producers;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.Producers
{
    public class ProducerHubService : IProducerHubService
    {
        private readonly IProducerHubRepository _producerHubRepository;

        public ProducerHubService(IProducerHubRepository producerHubRepository)
        {
            _producerHubRepository = producerHubRepository;
        }

        public Task NotifyProducerMessageArrivedToConsumer(string producerName, string consumerName, Guid messageId)
        {
            throw new NotImplementedException();
        }

        public Task NotifyProducerMessageFailed(string producerName, Guid messageId)
        {
            throw new NotImplementedException();
        }

        public Task NotifyProducerMessageSent(string producerName, Guid messageId)
        {
            throw new NotImplementedException();
        }
    }
}
