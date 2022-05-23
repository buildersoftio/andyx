using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Extensions.Authorization;
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
        private readonly ILogger<ProducerHub> _logger;
        private readonly IProducerHubRepository _producerHubRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantFactory _tenantFactory;
        private readonly IProducerFactory _producerFactory;
        private readonly IInboundMessageService _inboundMessageService;

        public ProducerHub(ILogger<ProducerHub> logger,
            IProducerHubRepository producerHubRepository,
            ITenantRepository tenantRepository,
            ITenantFactory tenantFactory,
            IProducerFactory producerFactory,
            IInboundMessageService inboundMessageService)
        {
            _logger = logger;
            _producerHubRepository = producerHubRepository;
            _tenantRepository = tenantRepository;
            _tenantFactory = tenantFactory;
            _producerFactory = producerFactory;

            _inboundMessageService = inboundMessageService;
        }

        public override Task OnConnectedAsync()
        {
            Producer producerToRegister;
            string clientConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;

            // authorization tokens
            string tenantToken = headers["x-andyx-tenant-authoriziation"];
            string componentToken = headers["x-andyx-component-authoriziation"];

            string tenant = headers["x-andyx-tenant"].ToString();
            string product = headers["x-andyx-product"].ToString();
            string component = headers["x-andyx-component"].ToString();
            string topic = headers["x-andyx-topic"].ToString();
            bool isPersistent = Boolean.Parse(headers["x-andyx-topic-is-persistent"]);


            string producerName = headers["x-andyx-producer"].ToString();

            _logger.LogInformation($"Producer '{producerName}' at {tenant}/{product}/{component}/{topic} requested connection");

            //check if the producer is already connected
            var connectedTenant = _tenantRepository.GetTenant(tenant);
            if (connectedTenant == null)
            {
                _logger.LogInformation($"Producer '{producerName}' failed to connect, tenant '{tenant}' does not exists");
                return OnDisconnectedAsync(new Exception($"There is no tenant registered with this name '{tenant}'"));
            }

            // check tenant token validation
            bool isTenantTokenValidated = _tenantRepository.ValidateTenantToken(tenant, tenantToken);
            if (isTenantTokenValidated != true)
            {
                _logger.LogInformation($"Producer '{producerName}' failed to connect, access is forbidden. Not authorized");
                return OnDisconnectedAsync(new Exception($"Producer '{producerName}' failed to connect, access is forbidden"));
            }

            var connectedProduct = _tenantRepository.GetProduct(tenant, product);
            if (connectedProduct == null)
            {
                if (connectedTenant.Settings.AllowProductCreation != true)
                {
                    _logger.LogInformation($"Producer '{producerName}' failed to connect, tenant '{tenant}' does not allow to create new product");
                    return OnDisconnectedAsync(new Exception($"There is no product registered with this name '{product}'. Tenant '{tenant}' does not allow to create new product"));
                }

                var productDetails = _tenantFactory.CreateProduct(product);
                _tenantRepository.AddProduct(tenant, product, productDetails);
            }

            var connectedComponent = _tenantRepository.GetComponent(tenant, product, component);
            if (connectedComponent == null)
            {
                var componentDetails = _tenantFactory.CreateComponent(component);
                _tenantRepository.AddComponent(tenant, product, component, componentDetails);
            }
            else
            {
                // check component token validation
                bool isComponentTokenValidated = _tenantRepository.ValidateComponentToken(tenant, product, component, componentToken, producerName, false);
                if (isComponentTokenValidated != true)
                {
                    _logger.LogInformation($"Producer '{producerName}' failed to connect, access is forbidden. Not authorized, check component token");
                    return OnDisconnectedAsync(new Exception($"Producer '{producerName}' failed to connect, access is forbidden, check component token"));
                }
            }

            var connectedTopic = _tenantRepository.GetTopic(tenant, product, component, topic);
            if (connectedTopic == null)
            {
                // Check if the component allows to create a new topic.
                connectedComponent = _tenantRepository.GetComponent(tenant, product, component);
                if (connectedComponent.Settings.AllowTopicCreation != true)
                {
                    _logger.LogInformation($"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component.");
                    return OnDisconnectedAsync(new Exception($"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component."));
                }

                var topicDetails = _tenantFactory.CreateTopic(topic, isPersistent);
                _tenantRepository.AddTopic(tenant, product, component, topic, topicDetails);
            }

            if (_producerHubRepository.GetProducerByProducerName(tenant, product, component, topic, producerName).Equals(default(KeyValuePair<string, Producer>)) != true)
            {
                _logger.LogWarning($"Producer '{producerName}' at {tenant}/{product}/{component}/{topic} is already connected");
                return OnDisconnectedAsync(new Exception($"There is a producer with name '{producerName}' at {tenant}/{product}/{component}/{topic} connected to this node"));
            }

            producerToRegister = _producerFactory.CreateProducer(tenant, product, component, topic, producerName);
            _producerHubRepository.AddProducer(clientConnectionId, producerToRegister);

            Clients.Caller.ProducerConnected(new Model.Producers.Events.ProducerConnectedDetails()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                Id = producerToRegister.Id.ToString(),
                ProducerName = producerName
            });
            _logger.LogInformation($"Producer '{producerName}' at {tenant}/{product}/{component}/{topic} is connected");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientConnectionId = Context.ConnectionId;
            Producer producerToRemove = _producerHubRepository.GetProducerById(clientConnectionId);

            // When the producer with the same name try to connect more than one.
            if (producerToRemove != null)
            {
                //storageHubService.DisconnectProducerAsync(producerToRemove);
                _producerHubRepository.RemoveProducer(clientConnectionId);

                _logger.LogInformation($"Producer '{producerToRemove.ProducerName}' at {producerToRemove.Tenant}/{producerToRemove.Product}/{producerToRemove.Component}/{producerToRemove.Topic} is disconnected");

                Clients.Caller.ProducerDisconnected(new Model.Producers.Events.ProducerDisconnectedDetails()
                {
                    Id = producerToRemove.Id
                });
            }
            return base.OnDisconnectedAsync(exception);
        }

        public void TransmitMessage(Message message)
        {
            _inboundMessageService.AcceptMessage(message);
            IncreaseMessageProducedCount();
        }

        public void TransmitMessages(List<Message> messages)
        {
            foreach (var message in messages)
            {
                _inboundMessageService.AcceptMessage(message);
            }

            IncreaseMessageProducedCount(messages.Count);
        }

        private void IncreaseMessageProducedCount(int count = 1)
        {
            string clientConnectionId = Context.ConnectionId;
            var producer = _producerHubRepository.GetProducerById(clientConnectionId);
            producer.CountMessagesProducedSinceConnected = producer.CountMessagesProducedSinceConnected + count;
        }
    }
}
