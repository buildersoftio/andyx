﻿using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Storages.Events.Messages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Consumers
{
    public class ConsumerHub : Hub<IConsumerHub>
    {
        private readonly ILogger<ConsumerHub> logger;
        private readonly IConsumerHubRepository consumerHubRepository;
        private readonly ITenantRepository tenantRepository;
        private readonly ITenantFactory tenantFactory;
        private readonly IConsumerFactory consumerFactory;
        private readonly IStorageHubService storageHubService;

        public ConsumerHub(ILogger<ConsumerHub> logger,
            IConsumerHubRepository consumerHubRepository,
            ITenantRepository tenantRepository,
            ITenantFactory tenantFactory,
            IConsumerFactory consumerFactory,
            IStorageHubService storageHubService)
        {
            this.logger = logger;
            this.consumerHubRepository = consumerHubRepository;
            this.tenantRepository = tenantRepository;
            this.tenantFactory = tenantFactory;
            this.consumerFactory = consumerFactory;
            this.storageHubService = storageHubService;
        }

        public override Task OnConnectedAsync()
        {
            Consumer consumerToRegister;
            string clientConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;

            string tenant = headers["x-andyx-tenant"].ToString();
            string product = headers["x-andyx-product"].ToString();
            string component = headers["x-andyx-component"].ToString();
            string topic = headers["x-andyx-topic"].ToString();
            bool isPersistent = Boolean.Parse(headers["x-andyx-topic-is-persistent"]);
            string consumerName = headers["x-andyx-consumer"].ToString();

            SubscriptionType subscriptionType = (SubscriptionType)Enum.Parse(typeof(SubscriptionType), headers["x-andyx-consumer-type"].ToString());
            InitialPosition initialPosition = (InitialPosition)Enum.Parse(typeof(InitialPosition), headers["x-andyx-consumer-initial-position"].ToString());

            logger.LogInformation($"Consumer '{consumerName}' and subscription type '{subscriptionType}' at {tenant}/{product}/{component}/{topic} requested connection");


            // check if the consumer is already connected
            if (tenantRepository.GetTenant(tenant) == null)
            {
                logger.LogInformation($"Consumer '{consumerName}' failed to connect, tenant '{tenant}' does not exists");

                return OnDisconnectedAsync(new Exception($"There is no tenant registered with this name '{tenant}'"));
            }

            var connectedProduct = tenantRepository.GetProduct(tenant, product);
            if (connectedProduct == null)
            {
                var productDetails = tenantFactory.CreateProduct(product);
                tenantRepository.AddProduct(tenant, product, productDetails);
                storageHubService.CreateProductAsync(tenant, productDetails);
            }
            else
            {
                storageHubService.UpdateProductAsync(tenant, connectedProduct);
            }

            var connectedComponent = tenantRepository.GetComponent(tenant, product, component);
            if (connectedComponent == null)
            {
                var componentDetails = tenantFactory.CreateComponent(component);
                tenantRepository.AddComponent(tenant, product, component, componentDetails);
                storageHubService.CreateComponentAsync(tenant, product, componentDetails);
            }
            else
            {
                storageHubService.UpdateComponentAsync(tenant, product, connectedComponent);
            }

            var connectedTopic = tenantRepository.GetTopic(tenant, product, component, topic);
            if (connectedTopic == null)
            {
                var topicDetails = tenantFactory.CreateTopic(topic, isPersistent);
                tenantRepository.AddTopic(tenant, product, component, topic, topicDetails);
                storageHubService.CreateTopicAsync(tenant, product, component, topicDetails);
            }
            else
            {
                storageHubService.UpdateTopicAsync(tenant, product, component, connectedTopic);
            }


            var consumerConencted = consumerHubRepository.GetConsumerByName(consumerName);
            if (consumerConencted != null)
            {
                if (subscriptionType == SubscriptionType.Exclusive)
                {
                    logger.LogWarning($"Consumer '{consumerName}' and subscription type '{subscriptionType}' at {tenant}/{product}/{component}/{topic} is already connected");
                    return OnDisconnectedAsync(new Exception($"There is a consumer with name '{consumerName}' and with type 'EXCLUSIVE' is connected to this node"));
                }

                if (subscriptionType == SubscriptionType.Failover)
                {
                    if (consumerConencted.Connections.Count >= 2)
                    {
                        logger.LogWarning($"Consumer '{consumerName}' and subscription type '{subscriptionType}' at {tenant}/{product}/{component}/{topic} is already connected with 2 instances");

                        return OnDisconnectedAsync(new Exception($"There are two consumers with name '{consumerName}' and with type 'Failover' are connected to this node"));
                    }
                }
            }

            consumerToRegister = consumerFactory.CreateConsumer(tenant, product, component, topic, consumerName, subscriptionType, initialPosition);
            consumerHubRepository.AddConsumer(consumerName, consumerToRegister);
            consumerHubRepository.AddConsumerConnection(consumerName, clientConnectionId);

            storageHubService.ConnectConsumerAsync(consumerToRegister);

            Clients.Caller.ConsumerConnected(new Model.Consumers.Events.ConsumerConnectedDetails()
            {
                Id = consumerToRegister.Id,
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                ConsumerName = consumerName,
                SubscriptionType = subscriptionType,
                InitialPosition = initialPosition
            });

            // if consumer is not persistent, do not store the message, just allow streaming
            if (isPersistent == true)
            {
                // Sent not acknoledged messages to this consumer (for exclusive and for the first shared consumer connected)
                if (subscriptionType == SubscriptionType.Exclusive || subscriptionType == SubscriptionType.Failover)
                    storageHubService.RequestUnacknowledgedMessagesConsumer(consumerToRegister);

                if (subscriptionType == SubscriptionType.Shared)
                {
                    if (consumerConencted == null)
                        storageHubService.RequestUnacknowledgedMessagesConsumer(consumerToRegister);
                }
            }
            logger.LogInformation($"Consumer '{consumerName}' and subscription type '{subscriptionType}' at {tenant}/{product}/{component}/{topic} is connected");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientConnectionId = Context.ConnectionId;
            Consumer consumerToRemove = consumerHubRepository.GetConsumerByConnectionId(clientConnectionId);

            // When the consumer as Exclusive with the same name try to connect
            if (consumerToRemove != null)
            {
                storageHubService.DisconnectConsumerAsync(consumerToRemove);

                consumerHubRepository.RemoveConsumerConnection(consumerToRemove.ConsumerName, clientConnectionId);
                consumerHubRepository.RemoveConsumer(consumerToRemove.ConsumerName);

                logger.LogInformation($"Consumer '{consumerToRemove.ConsumerName}' and subscription type '{consumerToRemove.SubscriptionType}' at {consumerToRemove.Tenant}/{consumerToRemove.Product}/{consumerToRemove.Component}/{consumerToRemove.Topic} is disconnected");

                Clients.Caller.ConsumerDisconnected(new Model.Consumers.Events.ConsumerDisconnectedDetails()
                {
                    Id = consumerToRemove.Id,
                    Tenant = consumerToRemove.Tenant,
                    Product = consumerToRemove.Product,
                    Component = consumerToRemove.Component,
                    Topic = consumerToRemove.Topic,
                    ConsumerName = consumerToRemove.ConsumerName
                });
            }
            return base.OnDisconnectedAsync(exception);
        }
        public async Task AcknowledgeMessage(MessageAcknowledgedDetails message)
        {
            // is a check to ignore if the topic is not persistent.
            if (tenantRepository.GetTopic(message.Tenant, message.Product, message.Component, message.Topic).TopicSettings.IsPersistent == true)
                await storageHubService.AcknowledgeMessage(message.Tenant, message.Product, message.Component, message.Topic, message.Consumer, message.IsAcknowledged, message.MessageId);
        }
    }
}
