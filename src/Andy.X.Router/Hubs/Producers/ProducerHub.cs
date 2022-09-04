﻿using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
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

        public ProducerHub(ILogger<ProducerHub> logger,
            IProducerHubRepository producerHubRepository,
            ITenantStateService tenantStateService,
            ICoreRepository coreRepository,
            ITenantFactory tenantFactory,
            IProducerFactory producerFactory,
            IInboundMessageService inboundMessageService,
            IClusterHubService clusterHubService)
        {
            _logger = logger;
            _producerHubRepository = producerHubRepository;
            _tenantStateService = tenantStateService;
            _coreRepository = coreRepository;
            _tenantFactory = tenantFactory;
            _producerFactory = producerFactory;

            _inboundMessageService = inboundMessageService;

            _clusterHubService = clusterHubService;
        }

        public override Task OnConnectedAsync()
        {
            Producer producerToRegister;
            string producerConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;

            // authorization tokens
            string tenantToken = headers["x-andyx-tenant-authoriziation"];
            string productToken = headers["x-andyx-product-authoriziation"];
            string componentToken = headers["x-andyx-component-authoriziation"];

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
                _logger.LogInformation($"Producer '{producerName}' failed to connect, tenant '{tenant}' does not exists");
                return OnDisconnectedAsync(new Exception($"There is no tenant registered with this name '{tenant}'"));
            }

            // check tenant token validation
            bool isTenantTokenValidated = _tenantStateService.ValidateTenantToken(_coreRepository, tenant, tenantToken, false);
            if (isTenantTokenValidated != true)
            {
                _logger.LogInformation($"Producer '{producerName}' failed to connect, access is forbidden. Not authorized");
                return OnDisconnectedAsync(new Exception($"Producer '{producerName}' failed to connect, access is forbidden"));
            }

            var connectedProduct = _tenantStateService.GetProduct(tenant, product);
            if (connectedProduct == null)
            {
                if (connectedTenant.Settings.IsProductAutomaticCreation != true)
                {
                    _logger.LogInformation($"Producer '{producerName}' failed to connect, tenant '{tenant}' does not allow to create new product");
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
                _logger.LogInformation($"Producer '{producerName}' failed to connect, access is forbidden. Not authorized, check product token");
                return OnDisconnectedAsync(new Exception($"Producer '{producerName}' failed to connect, access is forbidden, check product token"));
            }

            var connectedComponent = _tenantStateService.GetComponent(tenant, product, component);
            if (connectedComponent == null)
            {
                var componentDetails = _tenantFactory.CreateComponent(component);
                _tenantStateService.AddComponent(tenant, product, component, componentDetails);
            }
            else
            {
                // check component token validation
                var productFromState = _coreRepository.GetProduct(tenantFromState.Id, product);
                bool isComponentTokenValidated = _tenantStateService.ValidateComponentToken(_coreRepository, productFromState.Id, component, componentToken, producerName, false);
                if (isComponentTokenValidated != true)
                {
                    _logger.LogInformation($"Producer '{producerName}' failed to connect, access is forbidden. Not authorized, check component token");
                    return OnDisconnectedAsync(new Exception($"Producer '{producerName}' failed to connect, access is forbidden, check component token"));
                }
            }

            var connectedTopic = _tenantStateService.GetTopic(tenant, product, component, topic);
            if (connectedTopic == null)
            {
                // Check if the component allows to create a new topic.
                connectedComponent = _tenantStateService.GetComponent(tenant, product, component);
                if (connectedComponent.Settings.IsTopicAutomaticCreation != true)
                {
                    _logger.LogInformation($"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component.");
                    return OnDisconnectedAsync(new Exception($"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component."));
                }

                var topicDetails = _tenantFactory.CreateTopic(topic, topicDescription);
                _tenantStateService.AddTopic(tenant, product, component, topic, topicDetails);
            }

            if (_producerHubRepository.GetProducerByProducerName(tenant, product, component, topic, producerName).Equals(default(KeyValuePair<string, Producer>)) != true)
            {
                _logger.LogWarning($"Producer '{producerName}' at {tenant}/{product}/{component}/{topic} is already connected");
                return OnDisconnectedAsync(new Exception($"There is a producer with name '{producerName}' at {tenant}/{product}/{component}/{topic} connected to this node"));
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
