﻿using Buildersoft.Andy.X.Model.Storages.Requests.Producer;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Producers
{
    public interface IProducerHubService
    {
        Task NotifyProducerMessageSent(string producerName, Guid messageId);
        Task NotifyProducerMessageFailed(string producerName, Guid messageId);
        Task NotifyProducerMessageArrivedToConsumer(string producerName, string consumerName, Guid messageId);

        Task ConnectProducerFromOtherNode(NotifyProducerConnectionDetails notifyProducerConnectionDetails);
        Task DisconnectProducerFromOtherNode(NotifyProducerConnectionDetails notifyProducerConnectionDetails);

    }
}
