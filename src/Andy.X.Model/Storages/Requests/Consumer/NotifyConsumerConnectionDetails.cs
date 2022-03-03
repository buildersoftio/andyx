using Buildersoft.Andy.X.Model.Consumers;
using System;

namespace Buildersoft.Andy.X.Model.Storages.Requests.Consumer
{
    public class NotifyConsumerConnectionDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public Guid Id { get; set; }
        public string ConsumerName { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public InitialPosition InitialPosition { get; set; }
    }
    public enum ConnectionType
    {
        Connected,
        Disconnected
    }
}
