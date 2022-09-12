using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
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
        private readonly ITenantStateService _tenantStateService;
        private readonly ICoreRepository _coreRepository;
        private readonly ITenantFactory _tenantFactory;
        private readonly IProducerFactory _producerFactory;
        private readonly IInboundMessageService _inboundMessageService;

        private readonly IClusterHubService _clusterHubService;
        private readonly ICoreService _coreService;

        public ProducerHub(ILogger<ProducerHub> logger,
            IProducerHubRepository producerHubRepository,
            ITenantStateService tenantStateService,
            ICoreRepository coreRepository,
            ITenantFactory tenantFactory,
            IProducerFactory producerFactory,
            IInboundMessageService inboundMessageService,
            IClusterHubService clusterHubService,
            ICoreService coreService)
        {
            _logger = logger;
            _producerHubRepository = producerHubRepository;
            _tenantStateService = tenantStateService;
            _coreRepository = coreRepository;
            _tenantFactory = tenantFactory;
            _producerFactory = producerFactory;

            _inboundMessageService = inboundMessageService;

            _clusterHubService = clusterHubService;
            _coreService = coreService;
        }

        public override Task OnConnectedAsync()
        {
            Producer producerToRegister;
            string producerConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;

            // authorization tokens
            string tenantToken = headers["x-andyx-tenant-authoriziation"];
            tenantToken ??= "";
            string productToken = headers["x-andyx-product-authoriziation"];
            productToken ??= "";
            string componentToken = headers["x-andyx-component-authoriziation"];
            componentToken ??= "";

            string tenant = headers["x-andyx-tenant"].ToString();
            string product = headers["x-andyx-product"].ToString();
            string component = headers["x-andyx-component"].ToString();
            string topic = headers["x-andyx-topic"].ToString();
            string topicDescription = headers["x-andyx-topic-description"].ToString();

            string producerName = headers["x-andyx-producer-name"].ToString();

            _logger.LogInformation($"Producer '{producerName}' at {tenant}/{product}/{component}/{topic} requested connection");

            //check if the producer is already connected
            var connectedTenant = _tenantStateService.GetTenant(tenant);
            if (connectedTenant == null)
            {
                var message = $"Producer '{producerName}' failed to connect, tenant '{tenant}' does not exists";
                _logger.LogInformation(message);
                Clients.Caller.AndyOrderedDisconnect(message);
                return OnDisconnectedAsync(new Exception($"There is no tenant registered with this name '{tenant}'"));
            }

            // check tenant token validation
            bool isTenantTokenValidated = _tenantStateService.ValidateTenantToken(_coreRepository, tenant, tenantToken, false);
            if (isTenantTokenValidated != true)
            {
                var message = $"Producer '{producerName}' failed to connect, access is forbidden. Not authorized";
                _logger.LogInformation(message);
                Clients.Caller.AndyOrderedDisconnect(message);
                return OnDisconnectedAsync(new Exception($"Producer '{producerName}' failed to connect, access is forbidden"));
            }

            var connectedProduct = _tenantStateService.GetProduct(tenant, product);
            if (connectedProduct == null)
            {
                if (connectedTenant.Settings.IsProductAutomaticCreationAllowed != true)
                {
                    var message = $"Producer '{producerName}' failed to connect, tenant '{tenant}' does not allow to create new product";
                    _logger.LogInformation(message);
                    Clients.Caller.AndyOrderedDisconnect(message);
                    return OnDisconnectedAsync(new Exception($"There is no product registered with this name '{product}'. Tenant '{tenant}' does not allow to create new product"));
                }

                var productDetails = _tenantFactory.CreateProduct(product);
                _tenantStateService.AddProduct(tenant, product, productDetails);
            }

            // check product token validation
            var tenantFromState = _coreRepository.GetTenant(tenant);
            bool isProductTokenValidated = _tenantStateService.ValidateProductToken(_coreRepository, tenantFromState.Id, product, productToken, false);
            if (isProductTokenValidated != true)
            {
                string message = $"Producer '{producerName}' failed to connect, access is forbidden. Not authorized, check product token";
                _logger.LogInformation(message);
                Clients.Caller.AndyOrderedDisconnect(message);
                return OnDisconnectedAsync(new Exception($"Producer '{producerName}' failed to connect, access is forbidden, check product token"));
            }

            var connectedComponent = _tenantStateService.GetComponent(tenant, product, component);
            var productFromState = _coreRepository.GetProduct(tenantFromState.Id, product);
            if (connectedComponent == null)
            {
                var componentDetails = _tenantFactory.CreateComponent(component);
                _tenantStateService.AddComponent(tenant, product, component, componentDetails);
            }
            else
            {
                // check component token validation
                bool isComponentTokenValidated = _tenantStateService.ValidateComponentToken(_coreRepository, productFromState.Id, component, componentToken, producerName, false);
                if (isComponentTokenValidated != true)
                {
                    string message = $"Producer '{producerName}' failed to connect, access is forbidden. Not authorized, check component token";
                    _logger.LogInformation(message);
                    Clients.Caller.AndyOrderedDisconnect(message);
                    return OnDisconnectedAsync(new Exception($"Producer '{producerName}' failed to connect, access is forbidden, check component token"));
                }
            }

            var connectedTopic = _tenantStateService.GetTopic(tenant, product, component, topic);
            if (connectedTopic == null)
            {
                // Check if the component allows to create a new topic.
                connectedComponent = _tenantStateService.GetComponent(tenant, product, component);
                if (connectedComponent.Settings.IsTopicAutomaticCreationAllowed != true)
                {
                    string message = $"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component.";
                    _logger.LogInformation(message);
                    Clients.Caller.AndyOrderedDisconnect(message);
                    return OnDisconnectedAsync(new Exception($"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component."));
                }

                var topicDetails = _tenantFactory.CreateTopic(topic, topicDescription);
                _tenantStateService.AddTopic(tenant, product, component, topic, topicDetails);
            }

            // check if this is a new producer.
            var componentFromState = _coreRepository.GetComponent(tenantFromState.Id, productFromState.Id, component);
            var topicFromState = _coreRepository.GetTopic(componentFromState.Id, topic);
            var producerFromState = _coreRepository.GetProducer(topicFromState.Id, producerName);
            if (producerFromState == null)
            {
                var componentSettings = _coreRepository.GetComponentSettings(componentFromState.Id);
                if (componentSettings.IsProducerAutomaticCreationAllowed != true)
                {
                    var message = $"Component '{component}' does not allow to create a new producer {producerName} at '{tenant}/{product}/{component}'. To allow creating update property IsProducerAutomaticCreationAllowed at component settings.";
                    _logger.LogInformation(message);
                    Clients.Caller.AndyOrderedDisconnect(message);
                    return OnDisconnectedAsync(new Exception($"Component '{component}' does not allow to create a new producer {producerName} at '{tenant}/{product}/{component}'. To allow creating update property IsProducerAutomaticCreationAllowed at component settings."));

                }

                _coreService.CreateProducer(tenant, product, component, topic, producerName, "Created automatically with the client request", Model.Entities.Core.Producers.ProducerInstanceType.Multiple);
                producerFromState = _coreRepository.GetProducer(topicFromState.Id, producerName);
            }

            if (producerFromState.InstanceType == Model.Entities.Core.Producers.ProducerInstanceType.Single)
            {
                if (_producerHubRepository.GetProducerByProducerName(tenant, product, component, topic, producerName).Equals(default(KeyValuePair<string, Producer>)) != true)
                {
                    string message = $"Producer '{producerName}' at {tenant}/{product}/{component}/{topic} is already connected";
                    _logger.LogWarning(message);
                    Clients.Caller.AndyOrderedDisconnect(message);
                    return OnDisconnectedAsync(new Exception($"There is a producer with name '{producerName}' at {tenant}/{product}/{component}/{topic} connected to this node"));
                }
            }

            producerToRegister = _producerFactory.CreateProducer(tenant, product, component, topic, producerName);
            _producerHubRepository.AddProducer(producerConnectionId, producerToRegister);

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

            // notify other nodes in the cluster
            _clusterHubService.ConnectProducer_AllNodes(tenant, product, component, topic, producerConnectionId, producerName);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string producerConnectionId = Context.ConnectionId;
            Producer producerToRemove = _producerHubRepository.GetProducerById(producerConnectionId);

            // When the producer with the same name try to connect more than one.
            if (producerToRemove != null)
            {
                _clusterHubService.DisconnectProducer_AllNodes(producerToRemove.Tenant, producerToRemove.Product, producerToRemove.Component, producerToRemove.Topic, producerConnectionId);

                _producerHubRepository.RemoveProducer(producerConnectionId);
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
        }

        public void TransmitMessages(List<Message> messages)
        {
            foreach (var message in messages)
            {
                _inboundMessageService.AcceptMessage(message);
            }
        }
    }
}
