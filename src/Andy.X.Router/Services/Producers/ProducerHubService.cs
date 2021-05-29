using Buildersoft.Andy.X.Core.Abstractions.Services.Producers;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.Producers
{
    public class ProducerHubService : IProducerHubService
    {
        public Task NorifyProducerMessageArrivedToConsumer(string producerName, string consumerName, Guid messageId)
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
