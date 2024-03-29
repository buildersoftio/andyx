﻿using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Contexts.Subscriptions;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Entities.Subscriptions;
using Buildersoft.Andy.X.Model.Subscriptions;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Buildersoft.Andy.X.Router.Repositories.Subscriptions
{
    public class SubscriptionHubRepository : ISubscriptionHubRepository
    {
        private readonly ILogger<SubscriptionHubRepository> _logger;
        private ConcurrentDictionary<string, Subscription> _subscriptions;

        public SubscriptionHubRepository(ILogger<SubscriptionHubRepository> logger)
        {
            _logger = logger;
            _subscriptions = new ConcurrentDictionary<string, Subscription>();
        }

        public bool AddSubscription(string subscriptionId, Topic topic, Subscription subscription)
        {
            if (_subscriptions.ContainsKey(subscriptionId))
                return false;

            var isAdded = _subscriptions.TryAdd(subscriptionId, subscription);
            if (isAdded == true)
            {
                subscription.SubscriptionPositionContext = new SubscriptionPositionContext(subscription.Tenant,
                    subscription.Product, subscription.Component, subscription.Topic, subscription.SubscriptionName);

                // make sure that the db is created.
                subscription.SubscriptionPositionContext.ChangeTracker.AutoDetectChangesEnabled = false;
                subscription.SubscriptionPositionContext.Database.EnsureCreated();


                var position = (subscription.SubscriptionPositionContext as SubscriptionPositionContext).CurrentPosition.FirstOrDefault();
                if (position != null)
                    return true;

                if (subscription.InitialPosition == InitialPosition.Earliest)
                {
                    (subscription.SubscriptionPositionContext as SubscriptionPositionContext).CurrentPosition.Add(new SubscriptionPosition()
                    {
                        SubscriptionName = subscription.SubscriptionName,
                        MarkDeleteEntryPosition = 0,

                        // If there where some records deleted, the entry position should move to MarkDeleteEntryPosition of DEFAULT topic state.
                        ReadEntryPosition = topic.TopicStates.MarkDeleteEntryPosition,
                        PendingReadOperations = 0,
                        EntriesSinceLastUnacked = 0,
                        IsConnected = false,
                        CreatedDate = DateTimeOffset.Now,
                    });
                }
                else
                {

                    (subscription.SubscriptionPositionContext as SubscriptionPositionContext).CurrentPosition.Add(new SubscriptionPosition()
                    {
                        SubscriptionName = subscription.SubscriptionName,
                        MarkDeleteEntryPosition = 0,

                        ReadEntryPosition = topic.TopicStates.LatestEntryId,
                        PendingReadOperations = 0,
                        EntriesSinceLastUnacked = 0,
                        IsConnected = false,
                        CreatedDate = DateTimeOffset.Now,
                    });

                }
                (subscription.SubscriptionPositionContext as SubscriptionPositionContext).SaveChanges();
            }

            return isAdded;
        }

        public bool AddConsumer(string subscriptionId, string connectionId, Consumer consumer)
        {
            if (_subscriptions.ContainsKey(subscriptionId) != true)
                return false;

            return _subscriptions[subscriptionId].ConsumersConnected.TryAdd(connectionId, consumer);
        }

        public bool AddExternalConsumer(string subscriptionId, string connectionId, Consumer consumer)
        {
            if (_subscriptions.ContainsKey(subscriptionId) != true)
                return false;

            return _subscriptions[subscriptionId].ConsumerExternalConnected.TryAdd(connectionId, consumer);
        }

        public List<string> GetAllSubscriptionNames()
        {
            return _subscriptions.Keys.ToList();
        }

        public Consumer GetConsumerByConnectionId(string connectionId)
        {
            var subscription = _subscriptions.Values.Where(s => s.ConsumersConnected.ContainsKey(connectionId)).FirstOrDefault();
            if (subscription == null)
                return null;

            return subscription.ConsumersConnected[connectionId];
        }

        public Subscription GetSubscriptionByConnectionId(string connectionId)
        {
            return _subscriptions.Values.Where(s => s.ConsumersConnected.ContainsKey(connectionId)).FirstOrDefault();
        }

        public Dictionary<string, Subscription> GetSubscriptionsByTopic(string tenant, string product, string component, string topic)
        {
            try
            {
                return _subscriptions.Where(x => x.Value.Tenant == tenant
                    && x.Value.Product == product
                    && x.Value.Component == component
                    && x.Value.Topic == topic)
                        .ToDictionary(x => x.Key, x => x.Value);
            }

            catch (Exception)
            {
                // return Empty Dictionary, there is not consumer connected.
                return new Dictionary<string, Subscription>();
            }
        }


        public bool RemoveConsumerConnection(string subscriptionId, string connectionId)
        {
            if (_subscriptions.ContainsKey(subscriptionId) != true)
                return true;

            return _subscriptions[subscriptionId].ConsumersConnected.TryRemove(connectionId, out Consumer consumerRemoved);
        }

        public bool RemoveExternalConsumerConnection(string subscriptionId, string externalConenctionId)
        {
            if (_subscriptions.ContainsKey(subscriptionId) != true)
                return true;

            return _subscriptions[subscriptionId].ConsumersConnected.TryRemove(externalConenctionId, out Consumer consumerRemoved);
        }

        public ConcurrentDictionary<string, Consumer> GetConsumersBySubscrptionId(string subscriptionId)
        {
            if (_subscriptions.ContainsKey(subscriptionId) != true)
                return null;

            return _subscriptions[subscriptionId].ConsumersConnected;
        }

        public Subscription GetSubscriptionById(string subscriptionId)
        {
            if (_subscriptions.ContainsKey(subscriptionId) != true)
                return null;

            return _subscriptions[subscriptionId];
        }

        public List<SubscriptionActivity> GetAllSubscriptionActivities()
        {
            var results = new List<SubscriptionActivity>();

            foreach (var sub in _subscriptions)
            {
                var isActive = false;
                if (sub.Value.ConsumersConnected.Count != 0 || sub.Value.ConsumerExternalConnected.Count != 0)
                    isActive = true;
                results.Add(new SubscriptionActivity()
                {
                    Name = sub.Value.SubscriptionName,
                    Location = $"{sub.Value.Tenant}/{sub.Value.Product}/{sub.Value.Component}/{sub.Value.Topic}",
                    IsActive = isActive
                });
            }

            return results;
        }
    }
}
