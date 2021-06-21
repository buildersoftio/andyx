using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.Consumers;
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
            string consumerName = headers["x-andyx-consumer"].ToString();
            SubscriptionType consumerType = (SubscriptionType)Enum.Parse(typeof(SubscriptionType), headers["x-andyx-consumer-type"].ToString());

            logger.LogInformation($"ANDYX#CONSUMERS|{tenant}|{product}|{component}|{topic}|{consumerName}|{consumerType}|ASKED_TO_CONNECT");

            //check if the consumer is already connected

            if (tenantRepository.GetTenant(tenant) == null)
            {
                logger.LogInformation($"ANDYX#CONSUMERS|{tenant}|{product}|{component}|{topic}|{consumerName}|TENANT_DOES_NOT_EXISTS");
                return OnDisconnectedAsync(new Exception($"There is no tenant registered with this name '{tenant}'"));
            }

            if (tenantRepository.GetProduct(tenant, product) == null)
            {
                var productDetails = tenantFactory.CreateProduct(product);
                tenantRepository.AddProduct(tenant, product, productDetails);
                storageHubService.CreateProductAsync(tenant, productDetails);
            }

            if (tenantRepository.GetComponent(tenant, product, component) == null)
            {
                var componentDetails = tenantFactory.CreateComponent(component);
                tenantRepository.AddComponent(tenant, product, component, componentDetails);
                storageHubService.CreateComponentAsync(tenant, product, componentDetails);
            }

            if (tenantRepository.GetTopic(tenant, product, component, topic) == null)
            {
                var topicDetails = tenantFactory.CreateTopic(topic);
                tenantRepository.AddTopic(tenant, product, component, topic, topicDetails);
                storageHubService.CreateTopicAsync(tenant, product, component, topicDetails);
            }

            var consumerConencted = consumerHubRepository.GetConsumerByName(consumerName);
            if (consumerConencted != null)
            {
                if (consumerType == SubscriptionType.Exclusive)
                {
                    logger.LogInformation($"ANDYX#CONSUMERS|{tenant}|{product}|{component}|{topic}|{consumerName}|CONSUMER_EXCLUSIVE_ALREADY_CONNECTED");
                    return OnDisconnectedAsync(new Exception($"There is a consumer with name '{consumerName}' and with type 'EXCLUSIVE' is connected to this node"));
                }

                if (consumerType == SubscriptionType.Failover)
                {
                    if (consumerConencted.Connections.Count >= 2)
                    {
                        logger.LogInformation($"ANDYX#CONSUMERS|{tenant}|{product}|{component}|{topic}|{consumerName}|CONSUMER_FAILOVER_ALREADY_CONNECTED_2_INSTANCES");
                        return OnDisconnectedAsync(new Exception($"There are two consumers with name '{consumerName}' and with type 'Failover' are connected to this node"));
                    }
                }
            }

            consumerToRegister = consumerFactory.CreateConsumer(tenant, product, component, topic, consumerName, consumerType);
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
                ConsumerName = consumerName
            });

            logger.LogInformation($"ANDYX#CONSUMERS|{tenant}|{product}|{component}|{topic}|{consumerName}|{consumerType}|{consumerToRegister.Id}|CONNECTED");

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

                logger.LogInformation($"ANDYX#CONSUMERS|{consumerToRemove.Tenant}|{consumerToRemove.Product}|{consumerToRemove.Component}|{consumerToRemove.Topic}|{consumerToRemove.ConsumerName}|{consumerToRemove.SubscriptionType}|{consumerToRemove.Id}|DISCONNECTED");

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
    }
}
