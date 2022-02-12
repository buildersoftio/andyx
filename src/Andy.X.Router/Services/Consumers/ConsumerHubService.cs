using Buildersoft.Andy.X.Core.Abstractions.Hubs.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Core.Abstractions.Services.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Storages.Requests.Components;
using Buildersoft.Andy.X.Model.Storages.Requests.Tenants;
using Buildersoft.Andy.X.Router.Hubs.Consumers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.Consumers
{
    public class ConsumerHubService : IConsumerHubService
    {
        private readonly ILogger<ConsumerHubService> _logger;
        private readonly IHubContext<ConsumerHub, IConsumerHub> _hub;
        private readonly IConsumerHubRepository _consumerHubRepository;
        private readonly IStorageHubService _storageHubService;
        private readonly ITenantRepository _tenantRepository;

        private readonly ITenantService _tenantApiService;
        private readonly IComponentService _componentApiService;


        public ConsumerHubService(ILogger<ConsumerHubService> logger,
            IHubContext<ConsumerHub, IConsumerHub> hub,
            IConsumerHubRepository consumerHubRepository,
            IStorageHubService storageHubService,
            ITenantRepository tenantRepository,
            ITenantService tenantApiService,
            IComponentService componentApiService)
        {
            _logger = logger;
            _hub = hub;
            _consumerHubRepository = consumerHubRepository;
            _storageHubService = storageHubService;
            _tenantRepository = tenantRepository;
            _tenantApiService = tenantApiService;
            _componentApiService = componentApiService;
        }

        public async Task TransmitMessage(Message message, bool isStoredAlready = false)
        {
            if (isStoredAlready == false)
                message.ConsumersCurrentTransmitted = new List<string>();

            foreach (var consumer in _consumerHubRepository.GetConsumersByTopic(message.Tenant, message.Product, message.Component, message.Topic))
            {
                if (isStoredAlready == true)
                {
                    // If the message is sent to other nodes, do not send to the same consumer connected.
                    if (message.ConsumersCurrentTransmitted.Contains(consumer.Key))
                        continue;
                }

                if (consumer.Value.CurrentConnectionIndex >= consumer.Value.Connections.Count)
                    consumer.Value.CurrentConnectionIndex = 0;

                // This one is not needed, but let is stay for some time.
                if (consumer.Value.SubscriptionType == SubscriptionType.Exclusive || consumer.Value.SubscriptionType == SubscriptionType.Failover)
                {
                    consumer.Value.CurrentConnectionIndex = 0;
                }

                await _hub.Clients.Client(consumer.Value.Connections[consumer.Value.CurrentConnectionIndex]).MessageSent(new Model.Consumers.Events.MessageSentDetails()
                {
                    Id = message.Id,
                    Tenant = message.Tenant,
                    Product = message.Product,
                    Component = message.Component,
                    Topic = message.Topic,
                    MessageRaw = message.MessageRaw,
                    Headers = message.Headers,
                    SentDate = message.SentDate
                });

                consumer.Value.CurrentConnectionIndex++;

                if (isStoredAlready != true)
                    message.ConsumersCurrentTransmitted.Add(consumer.Key);
            }

            // If 'Message is Persistent', store it!
            var topicDetails = _tenantRepository.GetTopic(message.Tenant, message.Product, message.Component, message.Topic);
            if (topicDetails.TopicSettings.IsPersistent == true)
                if (isStoredAlready != true)
                    await _storageHubService.StoreMessage(message);
        }
        public async Task TransmitMessageToConsumer(ConsumerMessage consumerMessage)
        {
            string consumerId = $"{consumerMessage.Message.Tenant}{consumerMessage.Message.Product}{consumerMessage.Message.Component}{consumerMessage.Message.Topic}|{consumerMessage.Consumer}";
            var consumer = _consumerHubRepository.GetConsumerById(consumerId);
            if (consumer != null)
            {
                if (consumer.CurrentConnectionIndex >= consumer.Connections.Count)
                    consumer.CurrentConnectionIndex = 0;

                if (consumer.SubscriptionType == SubscriptionType.Exclusive || consumer.SubscriptionType == SubscriptionType.Failover)
                {
                    consumer.CurrentConnectionIndex = 0;
                }

                await _hub.Clients.Client(consumer.Connections[consumer.CurrentConnectionIndex]).MessageSent(new Model.Consumers.Events.MessageSentDetails()
                {
                    Id = consumerMessage.Message.Id,
                    Tenant = consumerMessage.Message.Tenant,
                    Product = consumerMessage.Message.Product,
                    Component = consumerMessage.Message.Component,
                    Topic = consumerMessage.Message.Topic,
                    MessageRaw = consumerMessage.Message.MessageRaw,
                    Headers = consumerMessage.Message.Headers,

                    SentDate = consumerMessage.Message.SentDate
                });

                consumer.CurrentConnectionIndex++;
            }
        }

        public async Task CreateComponentTokenToThisNode(CreateComponentTokenDetails createComponentTokenDetails)
        {
            if (_tenantRepository.GetComponentToken(createComponentTokenDetails.Tenant, createComponentTokenDetails.Product, createComponentTokenDetails.Component, createComponentTokenDetails.Token.Token) == null)
                _componentApiService.AddComponentToken(createComponentTokenDetails.Tenant, createComponentTokenDetails.Product, createComponentTokenDetails.Component, createComponentTokenDetails.Token, false);

            // send to other nodes....
            await _storageHubService.SendCreateComponentTokenStorage(createComponentTokenDetails);
        }
        public async Task CreateTenantTokenToThisNode(CreateTenantTokenDetails createTenantTokenDetails)
        {
            if (_tenantRepository.GetTenantToken(createTenantTokenDetails.Tenant, createTenantTokenDetails.Token.Token) == null)
                _tenantApiService.AddToken(createTenantTokenDetails.Tenant, createTenantTokenDetails.Token);

            // send to other nodes....
            await _storageHubService.SendCreateTenantTokenStorage(createTenantTokenDetails);
        }
        public async Task RevokeComponentTokenToThisNode(RevokeComponentTokenDetails revokeComponentTokenDetails)
        {
            if (_tenantRepository.GetComponentToken(revokeComponentTokenDetails.Tenant, revokeComponentTokenDetails.Product, revokeComponentTokenDetails.Component, revokeComponentTokenDetails.Token) != null)
                _componentApiService.RevokeComponentToken(revokeComponentTokenDetails.Tenant, revokeComponentTokenDetails.Product, revokeComponentTokenDetails.Component, revokeComponentTokenDetails.Token);

            // send to other nodes....
            await _storageHubService.SendRevokeComponentTokenStorage(revokeComponentTokenDetails);

        }
        public async Task RevokeTenantTokenToThisNode(RevokeTenantTokenDetails revokeTenantTokenDetails)
        {
            if (_tenantRepository.GetTenantToken(revokeTenantTokenDetails.Tenant, revokeTenantTokenDetails.Token) != null)
                _tenantApiService.RevokeToken(revokeTenantTokenDetails.Tenant, revokeTenantTokenDetails.Token);

            // send to other nodes....
            await _storageHubService.SendRevokeTenantTokenStorage(revokeTenantTokenDetails);
        }
        public async Task CreateTenantToThisNode(CreateTenantDetails createTenantDetails)
        {
            _logger.LogInformation($"Request to create tenant '{createTenantDetails.Name}' from other nodes");
            if (_tenantRepository.GetTenant(createTenantDetails.Name) == null)
            {
                _tenantApiService.CreateTenant(createTenantDetails.Name, createTenantDetails.TenantSettings);
                _logger.LogInformation($"Tenant' {createTenantDetails.Name}' has been created and linked, informing other nodes");
            }
            else
            {
                _logger.LogInformation($"Tenant' {createTenantDetails.Name}' already exists, ignoring the request");
            }

            // send to other nodes....
            await _storageHubService.SendCreateTenantStorage(createTenantDetails);
        }
    }
}
