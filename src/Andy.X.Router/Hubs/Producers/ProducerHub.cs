using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Producers;
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

        public ProducerHub(ILogger<ProducerHub> logger,
            IProducerHubRepository producerHubRepository,
            ITenantRepository tenantRepository,
            ITenantFactory tenantFactory,
            IProducerFactory producerFactory)
        {
            this.logger = logger;
            this.producerHubRepository = producerHubRepository;
            this.tenantRepository = tenantRepository;
            this.tenantFactory = tenantFactory;
            this.producerFactory = producerFactory;
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
            string producerName = headers["x-andyx-producer"].ToString();

            logger.LogInformation($"ANDYX#PRODUCERS|{tenant}|{product}|{component}|{topic}|{producerName}|ASKED_TO_CONNECT");

            //check if the producer is already connected

            if (tenantRepository.GetTenant(tenant) == null)
            {
                logger.LogInformation($"ANDYX#PRODUCERS|{tenant}|{product}|{component}|{topic}|{producerName}|TENANT_DOES_NOT_EXISTS");
                return OnDisconnectedAsync(new Exception($"There is no tenant registered with this name '{tenant}'"));
            }

            if (tenantRepository.GetProduct(tenant, product) == null)
            {
                // Create new product, store this product to ALL DATA STORAGES
                // TODO: Create a new DataStorage Service
                tenantRepository.AddProduct(tenant, product, tenantFactory.CreateProduct(product));
            }

            if (tenantRepository.GetComponent(tenant, product, component) == null)
            {
                // Create new component, store this product to ALL DATA STORAGES
                // TODO: Create a new DataStorage Service
                tenantRepository.AddComponent(tenant, product, component, tenantFactory.CreateComponent(component));
            }

            if (tenantRepository.GetTopic(tenant, product, component, topic) == null)
            {
                // Create new topic, store this product to ALL DATA STORAGES
                // TODO: Create a new DataStorage Service
                tenantRepository.AddTopic(tenant, product, component, topic, tenantFactory.CreateTopic(topic));
            }

            if (producerHubRepository.GetProducerByProducerName(tenant, product, component, topic, producerName).Equals(default(KeyValuePair<string, Producer>)) != true)
            {
                logger.LogInformation($"ANDYX#PRODUCERS|{tenant}|{product}|{component}|{topic}|{producerName}|PRODUCER_ALREADY_CONNECTED");
                return OnDisconnectedAsync(new Exception($"There is a producer with name '{producerName}' connected to this node"));
            }

            producerToRegister = producerFactory.CreateProducer(tenant, product, component, topic, producerName);
            producerHubRepository.AddProducer(clientConnectionId, producerToRegister);
            // Create new producer, store this product to ALL DATA STORAGES
            // TODO: Create a new DataStorage Service

            Clients.Caller.ProducerConnected(new Model.Producers.Events.ProducerConnectedDetails()
            {
                Id = producerToRegister.Id,
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                ProducerName = producerName
            });

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientConnectionId = Context.ConnectionId;
            Producer producerToRemove = producerHubRepository.GetProducerById(clientConnectionId);

            producerHubRepository.RemoveProducer(clientConnectionId);

            logger.LogInformation($"ANDYX#PRODUCERS|{producerToRemove.Tenant}|{producerToRemove.Product}|{producerToRemove.Component}|{producerToRemove.Topic}|{producerToRemove.ProducerName}|{producerToRemove.Id}|DISCONNECTED");

            Clients.Caller.ProducerDisconnected(new Model.Producers.Events.ProducerDisconnectedDetails()
            {
                Id = producerToRemove.Id
            });

            return base.OnDisconnectedAsync(exception);
        }
    }
}
