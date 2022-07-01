using Buildersoft.Andy.X.Core.Abstractions.Hubs.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Subscriptions;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Router.Hubs.Consumers;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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

            if (subscription.SubscriptionType == SubscriptionType.Unique
                || subscription.SubscriptionType == SubscriptionType.Failover)
            {
                await SendMessage(tenant, product, component, topic, message, subscription.ConsumersConnected.First().Key);
            }
            else
            {
                // Shared subscription...
                if (subscription.CurrentConnectionIndex >= subscription.ConsumersConnected.Count)
                    subscription.CurrentConnectionIndex = 0;

                await SendMessage(tenant, product, component, topic, message, subscription.ConsumersConnected.ElementAt(subscription.CurrentConnectionIndex).Key);

                subscription.CurrentConnectionIndex++;
            }
        }

        private async Task SendMessage(string tenant, string product, string component, string topic, Message message, string consumerKey)
        {
            await _hub.Clients.Client(consumerKey).MessageSent(new MessageSentDetails()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,

                EntryId = message.Entry,

                MessageId = message.MessageId,
                Headers = message.Headers,
                Payload = message.Payload,

                SentDate = message.SentDate
            });
        }
    }
}
