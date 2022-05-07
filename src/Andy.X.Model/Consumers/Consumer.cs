using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Consumers
{
    public class Consumer
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }

        public List<string> Connections { get; set; }
        public List<string> ExternalConnections { get; set; }

        public bool IsLocal { get; set; }

        // This property is used to send to the next shared consumer. (This property will replace the random)
        public int CurrentConnectionIndex { get; set; }

        public Guid Id { get; set; }
        public string ConsumerName { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public ConsumerSettings ConsumerSettings { get; set; }

        public DateTime ConnectedDate { get; set; }
        public long CountMessagesConsumedSinceConnected { get; set; }
        public long CountMessagesAcknowledgedSinceConnected { get; set; }
        public long CountMessagesUnacknowledgedSinceConnected { get; set; }

        public Consumer()
        {
            Connections = new List<string>();
            ExternalConnections = new List<string>();

            ConsumerSettings = new ConsumerSettings();
            ConnectedDate = DateTime.Now;
            CountMessagesAcknowledgedSinceConnected = 0;
            CountMessagesUnacknowledgedSinceConnected = 0;

            // is local -> flag if consumer is conencted to this node
            IsLocal = true;
        }
    }

    public class ConsumerSettings
    {
        public InitialPosition InitialPosition { get; set; }
        public ConsumerSettings()
        {
            InitialPosition = InitialPosition.Latest;
        }
    }

    public enum SubscriptionType
    {
        /// <summary>
        /// Only one reader
        /// </summary>
        Exclusive,
        /// <summary>
        /// One reader with one backup
        /// </summary>
        Failover,
        /// <summary>
        /// Shared to more than one reader.
        /// </summary>
        Shared
    }

    public enum InitialPosition
    {
        Earliest,
        Latest
    }
}
