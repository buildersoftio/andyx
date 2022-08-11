using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Extensions.Authorization;
using Buildersoft.Andy.X.IO.Services;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Consumers
{
    public class ConsumerHub : Hub<IConsumerHub>
    {
        private readonly ILogger<ConsumerHub> _logger;

        private readonly ISubscriptionHubRepository _subscriptionHubRepository;
        private readonly ITenantService _tenantRepository;

        private readonly ITenantFactory _tenantFactory;
        private readonly IConsumerFactory _consumerFactory;
        private readonly ISubscriptionFactory _subscriptionFactory;

        private readonly IOutboundMessageService _outboundMessageService;
        private readonly IInboundMessageService _inboundMessageService;
        private readonly StorageConfiguration _storageConfiguration;

        public ConsumerHub(ILogger<ConsumerHub> logger,
            ISubscriptionHubRepository subscriptionHubRepository,
            ITenantService tenantRepository,
            ITenantFactory tenantFactory,
            IConsumerFactory consumerFactory,
            ISubscriptionFactory subscriptionFactory,
            IOutboundMessageService outboundMessageService,
            IInboundMessageService inboundMessageService,
            StorageConfiguration storageConfiguration
            )
        {
            _logger = logger;

            _subscriptionHubRepository = subscriptionHubRepository;
            _tenantRepository = tenantRepository;

            _tenantFactory = tenantFactory;
            _consumerFactory = consumerFactory;
            _subscriptionFactory = subscriptionFactory;

            _outboundMessageService = outboundMessageService;
            _inboundMessageService = inboundMessageService;
            _storageConfiguration = storageConfiguration;
        }

        public override Task OnConnectedAsync()
        {
            Subscription subscriptionToRegister;
            string clientConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;

            // authorization tokens
            // TODO: Implement token validation
            string tenantToken = headers["x-andyx-tenant-authoriziation"];
            string componentToken = headers["x-andyx-component-authoriziation"];

            string tenant = headers["x-andyx-tenant"].ToString();
            string product = headers["x-andyx-product"].ToString();
            string component = headers["x-andyx-component"].ToString();
            string topic = headers["x-andyx-topic"].ToString();
            bool isPersistent = bool.Parse(headers["x-andyx-topic-is-persistent"]);
            string consumerName = headers["x-andyx-consumer-name"].ToString();

            string subscriptionName = headers["x-andyx-subscription-name"].ToString();
            SubscriptionMode subscriptionMode = (SubscriptionMode)Enum.Parse(typeof(SubscriptionMode), headers["x-andyx-subscription-mode"].ToString());
            SubscriptionType subscriptionType = (SubscriptionType)Enum.Parse(typeof(SubscriptionType), headers["x-andyx-subscription-type"].ToString());
            InitialPosition initialPosition = (InitialPosition)Enum.Parse(typeof(InitialPosition), headers["x-andyx-subscription-initial-position"].ToString());

            _logger.LogInformation($"Consumer '{consumerName}' requested connection to subscription '{subscriptionName}' at {tenant}/{product}/{component}/{topic}");

            // check if the consumer is already connected
            var connectedTenant = _tenantRepository.GetTenant(tenant);
            if (connectedTenant == null)
            {
                _logger.LogInformation($"Consumer '{consumerName}' failed to connect, tenant '{tenant}' does not exists");
                return OnDisconnectedAsync(new Exception($"There is no tenant registered with this name '{tenant}'"));
            }

            // check tenant token validation
            bool isTenantTokenValidated = _tenantRepository.ValidateTenantToken(tenant, tenantToken);
            if (isTenantTokenValidated != true)
            {
                _logger.LogInformation($"Consumer '{consumerName}' failed to connect, access is forbidden. Not authorized");
                return OnDisconnectedAsync(new Exception($"Consumer '{consumerName}' failed to connect, access is forbidden"));
            }

            var connectedProduct = _tenantRepository.GetProduct(tenant, product);
            if (connectedProduct == null)
            {
                if (connectedTenant.Settings.AllowProductCreation != true)
                {
                    _logger.LogInformation($"Consumer '{consumerName}' failed to connect, tenant '{tenant}' does not allow to create new product");
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
                bool isComponentTokenValidated = _tenantRepository.ValidateComponentToken(tenant, product, component, componentToken, consumerName, true);
                if (isComponentTokenValidated != true)
                {
                    _logger.LogInformation($"Consumer '{consumerName}' failed to connect, access is forbidden. Not authorized, check component token");
                    return OnDisconnectedAsync(new Exception($"Consumer '{consumerName}' failed to connect, access is forbidden, check component token"));
                }
            }

            var connectedTopic = _tenantRepository.GetTopic(tenant, product, component, topic);
            if (connectedTopic == null)
            {
                connectedComponent = _tenantRepository.GetComponent(tenant, product, component);
                if (connectedComponent.Settings.AllowTopicCreation != true)
                {
                    _logger.LogInformation($"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component.");
                    return OnDisconnectedAsync(new Exception($"Component '{component}' does not allow to create a new topic {topic} at '{tenant}/{product}/{component}'. To allow creating update property AllowTopicCreation at component."));
                }

                connectedTopic = _tenantFactory.CreateTopic(topic, isPersistent);
                _tenantRepository.AddTopic(tenant, product, component, topic, connectedTopic);
            }


            string subscriptionId = ConnectorHelper.GetSubcriptionId(tenant, product, component, topic, subscriptionName);
            var subscriptionConencted = _subscriptionHubRepository.GetSubscriptionById(subscriptionId);
            if (subscriptionConencted != null)
            {
                // check if the consumer has different subscription configuration, if yes, do not connect
                if (subscriptionConencted.SubscriptionMode != subscriptionMode)
                {
                    _logger.LogWarning($"Consumer '{consumerName}' can not connect to subscription '{subscriptionName}' at {tenant}/{product}/{component}/{topic}, because modes are different");
                    return OnDisconnectedAsync(new Exception($"Modes are different with subscription '{subscriptionName}'."));
                }

                if (subscriptionConencted.SubscriptionType != subscriptionType)
                {
                    _logger.LogWarning($"Consumer '{consumerName}' can not connect to subscription '{subscriptionName}' at {tenant}/{product}/{component}/{topic}, because types are different");
                    return OnDisconnectedAsync(new Exception($"Types are different with subscription '{subscriptionName}'."));
                }

                if (subscriptionConencted.InitialPosition != initialPosition)
                {
                    _logger.LogWarning($"Consumer '{consumerName}' can not connect to subscription '{subscriptionName}' at {tenant}/{product}/{component}/{topic}, because initial position is different");
                    return OnDisconnectedAsync(new Exception($"Initial position is different with subscription '{subscriptionName}'."));
                }

                if (subscriptionType == SubscriptionType.Unique)
                {
                    if (subscriptionConencted.ConsumersConnected.Count > 0 || subscriptionConencted.ConsumerExternalConnected.Count > 0)
                    {
                        _logger.LogWarning($"Consumer '{consumerName}' can not connect to subscription '{subscriptionName}' at {tenant}/{product}/{component}/{topic}, there is a consumer connected");
                        return OnDisconnectedAsync(new Exception($"There is a consumer already connected to subscription '{subscriptionName}'."));
                    }
                }

                if (subscriptionType == SubscriptionType.Failover)
                {
                    if (subscriptionConencted.ConsumersConnected.Count > 0 && subscriptionConencted.ConsumerExternalConnected.Count > 0)
                    {
                        _logger.LogWarning($"Consumer '{consumerName}' can not connect to subscription '{subscriptionName}' at {tenant}/{product}/{component}/{topic}, there are consumers connected in different nodes inside the cluster");
                        return OnDisconnectedAsync(new Exception($"There are consumers already connected to subscription '{subscriptionName}' in different nodes inside the cluster."));
                    }

                    if (subscriptionConencted.ConsumersConnected.Count >= 2 || subscriptionConencted.ConsumerExternalConnected.Count >= 2)
                    {
                        _logger.LogWarning($"Consumer '{consumerName}' can not connect to subscription '{subscriptionName}' at {tenant}/{product}/{component}/{topic}, there are consumers connected in different nodes inside the cluster");
                        return OnDisconnectedAsync(new Exception($"There are consumers already connected to subscription '{subscriptionName}' in different nodes inside the cluster."));
                    }
                }
            }

            subscriptionToRegister = _subscriptionFactory.CreateSubscription(tenant, product, component, topic, subscriptionName, subscriptionType, subscriptionMode, initialPosition);
            _tenantRepository.AddSubscriptionConfiguration(tenant, product, component, topic, subscriptionName, subscriptionToRegister);

            _subscriptionHubRepository.AddSubscription(subscriptionId, connectedTopic, subscriptionToRegister);

            var consumer = _consumerFactory.CreateConsumer(subscriptionName, consumerName);
            _subscriptionHubRepository.AddConsumer(subscriptionId, clientConnectionId, consumer);
            
            TenantIOService.TryCreateConsumerDirectory(tenant, product, component, topic, subscriptionName, consumerName);

            Task.Run(() => _outboundMessageService.AddSubscriptionTopicData(_subscriptionFactory.CreateSubscriptionTopicData(subscriptionToRegister,
                _storageConfiguration.OutboundFlushCurrentEntryPositionInMilliseconds,
                _storageConfiguration.OutboundBackgroundIntervalReadMessagesInMilliseconds)));

            // TODO: Inform other nodes that new consumer has been created.

            Clients.Caller.ConsumerConnected(new ConsumerConnectedDetails()
            {
                Id = consumer.Id,

                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                Subscription = subscriptionName,

                ConsumerName = consumerName
            });

            _logger.LogInformation($"Consumer '{consumerName}' is connected to subscription '{subscriptionName}' at {tenant}/{product}/{component}/{topic}");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientConnectionId = Context.ConnectionId;
            var subscription = _subscriptionHubRepository.GetSubscriptionByConnectionId(clientConnectionId);

            // When the consumer as Exclusive with the same name try to connect
            if (subscription != null)
            {
                string subscriptionId = ConnectorHelper.GetSubcriptionId(subscription.Tenant, subscription.Product, subscription.Component, subscription.Topic, subscription.SubscriptionName);
                Consumer consumerToRemove = _subscriptionHubRepository.GetConsumerByConnectionId(clientConnectionId);

                _outboundMessageService.StoreCurrentPositionAsync(subscriptionId);

                _subscriptionHubRepository.RemoveConsumerConnection(subscriptionId, clientConnectionId);
                _logger.LogInformation($"Consumer '{consumerToRemove.Name}' is disconencted from subscription '{subscription.SubscriptionName}' at {subscription.Tenant}/{subscription.Product}/{subscription.Component}/{subscription.Topic}");

                Clients.Caller.ConsumerDisconnected(new ConsumerDisconnectedDetails()
                {
                    Id = consumerToRemove.Id,
                    Tenant = subscription.Tenant,
                    Product = subscription.Product,
                    Component = subscription.Component,
                    Topic = subscription.Topic,
                    ConsumerName = consumerToRemove.Name
                });

                _outboundMessageService.StopOutboundMessageServiceForSubscription(subscriptionId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task AcknowledgeMessage(MessageAcknowledgedDetails message)
        {
            var clientConnectionId = Context.ConnectionId;
            var subscription = _subscriptionHubRepository.GetSubscriptionByConnectionId(clientConnectionId);
            var subscriptionId = ConnectorHelper.GetSubcriptionId(subscription.Tenant, subscription.Product, subscription.Component, subscription.Topic, subscription.SubscriptionName);
            var messageAcknowledgement = (MessageAcknowledgement)message.Acknowledgement;

            if (subscription.SubscriptionMode == SubscriptionMode.Resilient)
            {
                switch (messageAcknowledgement)
                {
                    case MessageAcknowledgement.Acknowledged:
                    case MessageAcknowledgement.Skipped:
                        await _outboundMessageService.SendNextMessage(subscriptionId, message.EntryId);
                        break;
                    case MessageAcknowledgement.Unacknowledged:
                        await _outboundMessageService.SendSameMessage(subscriptionId, message.EntryId);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (messageAcknowledgement)
                {
                    case MessageAcknowledgement.Acknowledged:
                    case MessageAcknowledgement.Skipped:
                        if (_outboundMessageService.CheckIfUnackedMessagesExists(subscriptionId, message.EntryId) == true)
                        {
                            _outboundMessageService.DeleteEntryOfUnackedMessages(subscriptionId);
                        }
                        else
                        {
                            await _outboundMessageService.UpdateCurrentPosition(subscriptionId, message.EntryId);
                        }
                        break;
                    case MessageAcknowledgement.Unacknowledged:
                        if (_outboundMessageService.CheckIfUnackedMessagesExists(subscriptionId, message.EntryId) == true)
                        {
                            _outboundMessageService.DeleteEntryOfUnackedMessages(subscriptionId);
                        }
                        else
                        {
                            await _outboundMessageService.UpdateCurrentPosition(subscriptionId, message.EntryId);
                        }

                        _inboundMessageService.AcceptUnacknowledgedMessage(subscription.Tenant, subscription.Product, subscription.Component, subscription.Topic, subscription.SubscriptionName, message);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
