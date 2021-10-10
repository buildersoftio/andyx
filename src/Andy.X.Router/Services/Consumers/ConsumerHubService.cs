using Buildersoft.Andy.X.Core.Abstractions.Hubs.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Router.Hubs.Consumers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.Consumers
{
    public class ConsumerHubService : IConsumerHubService
    {
        private readonly ILogger<ConsumerHubService> logger;
        private readonly IHubContext<ConsumerHub, IConsumerHub> hub;
        private readonly IConsumerHubRepository consumerHubRepository;
        private readonly IStorageHubService storageHubService;
        private readonly ITenantRepository tenantRepository;

        public ConsumerHubService(ILogger<ConsumerHubService> logger,
            IHubContext<ConsumerHub, IConsumerHub> hub,
            IConsumerHubRepository consumerHubRepository,
            IStorageHubService storageHubService,
            ITenantRepository tenantRepository)
        {
            this.logger = logger;
            this.hub = hub;
            this.consumerHubRepository = consumerHubRepository;
            this.storageHubService = storageHubService;
            this.tenantRepository = tenantRepository;
        }

        public async Task TransmitMessage(Message message, bool isStoredAlready = false)
        {
            if (isStoredAlready == false)
                message.ConsumersCurrentTransmitted = new List<string>();

            foreach (var consumer in consumerHubRepository.GetConsumersByTopic(message.Tenant, message.Product, message.Component, message.Topic))
            {
                if (isStoredAlready == true)
                {
                    // If the message is sent to other nodes, do not send to the same consumer connected.
                    if (message.ConsumersCurrentTransmitted.Contains(consumer.Key))
                        continue;
                }

                int index = new Random().Next(consumer.Value.Connections.Count);
                if (consumer.Value.SubscriptionType == SubscriptionType.Exclusive || consumer.Value.SubscriptionType == SubscriptionType.Failover)
                {
                    index = 0;
                }

                await hub.Clients.Client(consumer.Value.Connections[index]).MessageSent(new Model.Consumers.Events.MessageSentDetails()
                {
                    Id = message.Id,
                    Tenant = message.Tenant,
                    Product = message.Product,
                    Component = message.Component,
                    Topic = message.Topic,
                    MessageRaw = message.MessageRaw
                });

                if (isStoredAlready != true)
                    message.ConsumersCurrentTransmitted.Add(consumer.Key);
            }

            // If 'Message is Persistent', store it!
            var topicDetails = tenantRepository.GetTopic(message.Tenant, message.Product, message.Component, message.Topic);
            if (topicDetails.TopicSettings.IsPersistent == true)
                if (isStoredAlready != true)
                    await storageHubService.StoreMessage(message);
        }

        public async Task TransmitMessageToConsumer(ConsumerMessage consumerMessage)
        {
            var consumer = consumerHubRepository.GetConsumerByName(consumerMessage.Consumer);
            if (consumer != null)
            {
                int index = new Random().Next(consumer.Connections.Count);
                if (consumer.SubscriptionType == SubscriptionType.Exclusive || consumer.SubscriptionType == SubscriptionType.Failover)
                {
                    index = 0;
                }

                await hub.Clients.Client(consumer.Connections[index]).MessageSent(new Model.Consumers.Events.MessageSentDetails()
                {
                    Id = consumerMessage.Message.Id,
                    Tenant = consumerMessage.Message.Tenant,
                    Product = consumerMessage.Message.Product,
                    Component = consumerMessage.Message.Component,
                    Topic = consumerMessage.Message.Topic,
                    MessageRaw = consumerMessage.Message.MessageRaw
                });
            }
        }
    }
}
