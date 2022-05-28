using Buildersoft.Andy.X.Core.Abstractions.Hubs.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Subscriptions;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Buildersoft.Andy.X.Router.Hubs.Consumers;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.Subscriptions
{
    public class SubscriptionHubService : ISubscriptionHubService
    {
        private readonly ILogger<SubscriptionHubService> _logger;
        private readonly IHubContext<ConsumerHub, IConsumerHub> _hub;
        private readonly ISubscriptionHubRepository _subscriptionHubRepository;


        public SubscriptionHubService(ILogger<SubscriptionHubService> logger,
            IHubContext<ConsumerHub, IConsumerHub> hub,
            ISubscriptionHubRepository subscriptionHubRepository)
        {
            _logger = logger;

            _hub = hub;
            _subscriptionHubRepository = subscriptionHubRepository;
        }

        public async Task TransmitMessage(string tenant, string product, string component, string topic, string subscriptionName, Message message)
        {
            var subscription = _subscriptionHubRepository.GetSubscriptionById(ConnectorHelper.GetSubcriptionId(tenant, product, component, topic, subscriptionName));
            if (subscription.SubscriptionType == Model.Subscriptions.SubscriptionType.Unique)
            {
                await _hub.Clients.Client(subscription.ConsumersConnected.First().Key).MessageSent(new MessageSentDetails()
                {
                    Tenant = tenant,
                    Product = product,
                    Component = component,
                    Topic = topic,

                    LedgerId = message.LedgerId,
                    EntryId = message.Id,

                    MessageId = message.MessageId,
                    Headers = message.Headers.JsonToObject<Dictionary<string, string>>(),
                    Payload = message.Payload,

                    SentDate = message.SentDate
                });
            };

        }
    }
}
