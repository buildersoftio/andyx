using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Model.Clusters.Events;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class ConsumerEventHandler
    {
        private const string CONSUMER_KEY = "ext_";


        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly ISubscriptionHubRepository _subscriptionHubRepository;
        private readonly ISubscriptionFactory _subscriptionFactory;
        private readonly IConsumerFactory _consumerFactory;

        public ConsumerEventHandler(NodeClusterEventService nodeClusterEventService,
            ISubscriptionHubRepository subscriptionHubRepository,
            ISubscriptionFactory subscriptionFactory,
            IConsumerFactory consumerFactory)
        {
            _nodeClusterEventService = nodeClusterEventService;
            _subscriptionHubRepository = subscriptionHubRepository;
            _subscriptionFactory = subscriptionFactory;
            _consumerFactory = consumerFactory;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.ConsumerConnected += NodeClusterEventService_ConsumerConnected;
            _nodeClusterEventService.ConsumerDisconnected += NodeClusterEventService_ConsumerDisconnected;
        }

        private void NodeClusterEventService_ConsumerDisconnected(ConsumerDisconnectedArgs obj)
        {
            try
            {
                var consumerKey = CONSUMER_KEY + obj.ConsumerConnectionId;

                string subscriptionId = ConnectorHelper.GetSubcriptionId(obj.Tenant, obj.Product, obj.Component,
                    obj.Topic, obj.Subscription);

                Consumer consumerToRemove = _subscriptionHubRepository.GetConsumerByConnectionId(consumerKey);

                _subscriptionHubRepository.RemoveExternalConsumerConnection(subscriptionId, consumerKey);
            }
            catch (Exception)
            {
                // TODO: Log later
            }
        }

        private void NodeClusterEventService_ConsumerConnected(ConsumerConnectedArgs obj)
        {
            try
            {
                var consumerKey = CONSUMER_KEY + obj.ConsumerConnectionId;

                string subscriptionId = ConnectorHelper.GetSubcriptionId(obj.Tenant, obj.Product, obj.Component,
                    obj.Topic, obj.SubscriptionDetails.SubscriptionName);

                _subscriptionFactory.CreateSubscription(obj.Tenant, obj.Product, obj.Component,
                    obj.Topic, obj.SubscriptionDetails.SubscriptionName, obj.SubscriptionDetails.SubscriptionType,
                    obj.SubscriptionDetails.SubscriptionMode, obj.SubscriptionDetails.InitialPosition);

                var consumer = _consumerFactory.CreateConsumer(obj.SubscriptionDetails.SubscriptionName, obj.Consumer);
                _subscriptionHubRepository.AddExternalConsumer(subscriptionId, consumerKey, consumer);
            }
            catch (Exception)
            {
                // TODO: Log later
            }
        }
    }
}
