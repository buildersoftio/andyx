﻿using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Subscriptions;
using System;

namespace Buildersoft.Andy.X.Core.Factories.Subscriptions
{
    public class SubscriptionFactory : ISubscriptionFactory
    {
        public Subscription CreateSubscription(string tenant, string product, string component, string topic, string subscriptionName, SubscriptionType subscriptionType, SubscriptionMode subscriptionMode, InitialPosition initialPosition)
        {
            return new Subscription()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,

                SubscriptionName = subscriptionName,
                SubscriptionType = subscriptionType,
                SubscriptionMode = subscriptionMode,
                InitialPosition = initialPosition,

                CreatedDate = DateTimeOffset.Now
            };
        }

        public SubscriptionTopicData CreateSubscriptionTopicData(Subscription subscription, int flushCurrentPositionTimer, int backgroundIntervalReadMessages)
        {
            return new SubscriptionTopicData(flushCurrentPositionTimer, backgroundIntervalReadMessages)
            {
                Subscription = subscription
            };
        }

        public MessageAcknowledgementFileContent CreateUnackAcknowledgedMessageContent(string tenant, string product, string component, string topic, string subscription, MessageAcknowledgedDetails message)
        {
            return new MessageAcknowledgementFileContent()
            {
                Tenant = tenant,
                Product = product,
                Component = component,
                Topic = topic,
                Subscription = subscription,
                EntryId = message.EntryId,
                CreatedDate = DateTimeOffset.Now
            };
        }
    }
}
