using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Producers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Producers
{
    public class ProducerHub : Hub<IProducerHub>
    {
        private readonly ILogger<ProducerHub> logger;
        private readonly IProducerHubRepository producerHubRepository;
        private readonly ITenantRepository tenantRepository;
        private readonly ITenantFactory tenantFactory;
        private readonly IProducerFactory producerFactory;
        private readonly IStorageHubService storageHubService;
        private readonly IConsumerHubService consumerHubService;

        public ProducerHub(ILogger<ProducerHub> logger,
            IProducerHubRepository producerHubRepository,
            ITenantRepository tenantRepository,
            ITenantFactory tenantFactory,
            IProducerFactory producerFactory,
            IStorageHubService storageHubService,
            IConsumerHubService consumerHubService)
        {
            this.logger = logger;
            this.producerHubRepository = producerHubRepository;
            this.tenantRepository = tenantRepository;
            this.tenantFactory = tenantFactory;
            this.producerFactory = producerFactory;
            this.storageHubService = storageHubService;
            this.consumerHubService = consumerHubService;
        }

        public override Task OnConnectedAsync()
        {
            Producer producerToRegister;
            string clientConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;

            string tenant = headers["x-andyx-tenant"].ToString();
            string product = headers["x-andyx-product"].ToString();
            string component = headers["x-andyx-component"].ToString();
            string topic = headers["x-andyx-topic"].ToString();
            bool isPersistent = Boolean.Parse(headers["x-andyx-topic-is-persistent"]);

            string producerName = headers["x-andyx-producer"].ToString();

            logger.LogInformation($"Producer '{producerName}' at {tenant}/{product}/{component}/{topic} requested connection");

            //check if the producer is already connected

            var connectedTenant = tenantRepository.GetTenant(tenant);
            if (connectedTenant == null)
            {
                logger.LogInformation($"Producer '{producerName}' failed to connect, tenant '{tenant}' does not exists");
                return OnDisconnectedAsync(new Exception($"There is no tenant registered with this name '{tenant}'"));
            }

            var connectedProduct = tenantRepository.GetProduct(tenant, product);
            if (connectedProduct == null)
            {
                if (connectedTenant.IsProductAutoCreate != true)
                {
                    logger.LogInformation($"Producer '{producerName}' failed to connect, tenant '{tenant}' does not allow to create new product");
                    return OnDisconnectedAsync(new Exception($"There is no product registered with this name '{product}'. Tenant '{tenant}' does not allow to create new product"));
                }

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
                // Check if the component allows to create a new topic.
                connectedComponent = tenantRepository.GetComponent(tenant, product, component);
                if (connectedComponent.AllowTopicCreation != true)
                {
                    logger.LogInformation($"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component.");
                    return OnDisconnectedAsync(new Exception($"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component."));
                }

                var topicDetails = tenantFactory.CreateTopic(topic, isPersistent);
                tenantRepository.AddTopic(tenant, product, component, topic, topicDetails);
                storageHubService.CreateTopicAsync(tenant, product, component, topicDetails);
            }
            else
            {
                storageHubService.UpdateTopicAsync(tenant, product, component, connectedTopic);
            }

            if (producerHubRepository.GetProducerByProducerName(tenant, product, component, topic, producerName).Equals(default(KeyValuePair<string, Producer>)) != true)
            {
                logger.LogWarning($"Producer '{producerName}' at {tenant}/{product}/{component}/{topic} is already connected");
                return OnDisconnectedAsync(new Exception($"There is a producer with name '{producerName}' at {tenant}/{product}/{component}/{topic} connected to this node"));
            }

            producerToRegister = producerFactory.CreateProducer(tenant, product, component, topic, producerName);
            producerHubRepository.AddProducer(clientConnectionId, producerToRegister);
            storageHubService.ConnectProducerAsync(producerToRegister);

            Clients.Caller.ProducerConnected(new Model.Producers.Events.ProducerConnectedDetails()
            {
                Id = producerToRegister.Id,
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                ProducerName = producerName
            });
            logger.LogInformation($"Producer '{producerName}' at {tenant}/{product}/{component}/{topic} is connected");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientConnectionId = Context.ConnectionId;
            Producer producerToRemove = producerHubRepository.GetProducerById(clientConnectionId);

            // When the producer with the same name try to connect more than one.
            if (producerToRemove != null)
            {
                storageHubService.DisconnectProducerAsync(producerToRemove);
                producerHubRepository.RemoveProducer(clientConnectionId);

                logger.LogInformation($"Producer '{producerToRemove.ProducerName}' at {producerToRemove.Tenant}/{producerToRemove.Product}/{producerToRemove.Component}/{producerToRemove.Topic} is disconnected");

                Clients.Caller.ProducerDisconnected(new Model.Producers.Events.ProducerDisconnectedDetails()
                {
                    Id = producerToRemove.Id
                });
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task TransmitMessage(Message messageDetails)
        {
            await consumerHubService.TransmitMessage(messageDetails);
        }
    }
}
