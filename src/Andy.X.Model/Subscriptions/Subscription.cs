using Buildersoft.Andy.X.Model.Consumers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Model.Subscriptions
{
    public class Subscription
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }

        public string SubscriptionName { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public SubscriptionMode SubscriptionMode { get; set; }
        public InitialPosition InitialPosition { get; set; }

        // list of connected consumers on this node
        public ConcurrentDictionary<string, Consumer> ConsumersConnected { get; set; }
        public ConcurrentDictionary<string, Consumer> ConsumerExternalConnected { get; set; }

        public int CurrentConnectionIndex { get; set; }
        public DbContext SubscriptionPositionContext { get; set; }
        public DbContext SubscriptionAcknowledgementContext { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public Subscription()
        {
            ConsumersConnected = new ConcurrentDictionary<string, Consumer>();
            ConsumerExternalConnected = new ConcurrentDictionary<string, Consumer>();
            CurrentConnectionIndex = 0;
        }
    }

    public enum SubscriptionType
    {
        /// <summary>
        /// Only one consumer
        /// </summary>
        Unique,
        /// <summary>
        /// One consumer with one backup
        /// </summary>
        Failover,
        /// <summary>
        /// Shared to more than one consumer.
        /// </summary>
        Shared
    }

    public enum InitialPosition
    {
        Earliest,
        Latest
    }

    public enum SubscriptionMode
    {
        /// <summary>
        /// Durable
        /// </summary>
        Resilient,

        /// <summary>
        /// Non Durable
        /// </summary>
        NonResilient
    }
}
