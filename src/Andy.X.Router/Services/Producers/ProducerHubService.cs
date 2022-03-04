using Buildersoft.Andy.X.Core.Abstractions.Repositories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Producers;
using Buildersoft.Andy.X.Model.Storages.Requests.Producer;
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


        public Task ConnectProducerFromOtherNode(NotifyProducerConnectionDetails notifyProducerConnectionDetails)
        {
            _producerHubRepository.AddProducer($"EXTERNAL-{Guid.NewGuid()}", new Model.Producers.Producer()
            {
                IsLocal = false,
                Component = notifyProducerConnectionDetails.Component,
                Id = notifyProducerConnectionDetails.Id,
                ProducerName = notifyProducerConnectionDetails.ProducerName,
                Product = notifyProducerConnectionDetails.Product,
                Tenant = notifyProducerConnectionDetails.Tenant,
                Topic = notifyProducerConnectionDetails.Topic
            });


            return Task.CompletedTask;
        }

        public Task DisconnectProducerFromOtherNode(NotifyProducerConnectionDetails notifyProducerConnectionDetails)
        {
            var producerExternalToRemove = _producerHubRepository.GetProducerByProducerName(notifyProducerConnectionDetails.Tenant,
                    notifyProducerConnectionDetails.Product,
                    notifyProducerConnectionDetails.Component,
                    notifyProducerConnectionDetails.Topic,
                    notifyProducerConnectionDetails.ProducerName);

            if (producerExternalToRemove.Value != null)
            {
                if (producerExternalToRemove.Key.StartsWith("EXTERNAL"))
                    _producerHubRepository.RemoveProducer(producerExternalToRemove.Key);
            }

            return Task.CompletedTask;
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
