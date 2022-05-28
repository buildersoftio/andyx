using Buildersoft.Andy.X.Model.Subscriptions;
using System;

namespace Buildersoft.Andy.X.Model.Consumers.Events
{
    public class ConsumerDisconnectedDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }

        public Guid Id { get; set; }
        public string ConsumerName { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
    }
}
