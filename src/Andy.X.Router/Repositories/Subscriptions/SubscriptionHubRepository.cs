using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.App.Repositories.Memory;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.Core.Contexts.Subscriptions;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Entities.Subscriptions;
using Buildersoft.Andy.X.Model.Subscriptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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

        public bool AddSubscription(string subscriptionId, Subscription subscription)
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

                subscription.SubscriptionAcknowledgementContext = new MessageAcknowledgementContext(subscription.Tenant,
                    subscription.Product, subscription.Component, subscription.Topic, subscription.SubscriptionName);

                //  ensure that db is created.
                subscription.SubscriptionAcknowledgementContext.ChangeTracker.AutoDetectChangesEnabled = false;
                subscription.SubscriptionAcknowledgementContext.Database.EnsureCreated();

                // check if record exists.
                var position = (subscription.SubscriptionPositionContext as SubscriptionPositionContext).CurrentPosition.FirstOrDefault();
                if (position != null)
                    return true;

                if (subscription.InitialPosition == InitialPosition.Earliest)
                {
                    (subscription.SubscriptionPositionContext as SubscriptionPositionContext).CurrentPosition.Add(new SubscriptionPosition()
                    {
                        SubscriptionName = subscription.SubscriptionName,
                        MarkDeleteLedgerPosition = 1,
                        MarkDeleteEntryPosition = -1,

                        ReadLedgerPosition = 1,
                        ReadEntryPosition = 0,
                        PendingReadOperations = 0,
                        EntriesSinceLastUnacked = 0,
                        IsConnected = false,
                        CreatedDate = DateTimeOffset.Now,
                    });
                }
                else
                {
                    // get the ledger log db file
                    if (File.Exists(TenantLocations.GetTopicLedgerLogFile(subscription.Tenant, subscription.Product, subscription.Component, subscription.Topic)) != true)
                    {
                        (subscription.SubscriptionPositionContext as SubscriptionPositionContext).CurrentPosition.Add(new SubscriptionPosition()
                        {
                            SubscriptionName = subscription.SubscriptionName,
                            MarkDeleteLedgerPosition = 1,
                            MarkDeleteEntryPosition = -1,

                            ReadLedgerPosition = 1,
                            ReadEntryPosition = 0,
                            PendingReadOperations = 0,
                            EntriesSinceLastUnacked = 0,
                            IsConnected = false,
                            CreatedDate = DateTimeOffset.Now,
                        });
                    }
                    else
                    {
                        var ledgerRepository = new LedgerRepository(subscription.Tenant, subscription.Product, subscription.Component, subscription.Topic);
                        var ledger = ledgerRepository.GetLastestLedgerData();
                        if (ledger == null)
                        {
                            (subscription.SubscriptionPositionContext as SubscriptionPositionContext).CurrentPosition.Add(new SubscriptionPosition()
                            {
                                SubscriptionName = subscription.SubscriptionName,
                                MarkDeleteLedgerPosition = 1,
                                MarkDeleteEntryPosition = -1,

                                ReadLedgerPosition = 1,
                                ReadEntryPosition = 0,
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
                                MarkDeleteLedgerPosition = 1,
                                MarkDeleteEntryPosition = -1,

                                ReadLedgerPosition = ledger.Id,
                                ReadEntryPosition = ledger.Entries,
                                PendingReadOperations = 0,
                                EntriesSinceLastUnacked = 0,
                                IsConnected = false,
                                CreatedDate = DateTimeOffset.Now,
                            });
                        }
                    }
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
    }
}
