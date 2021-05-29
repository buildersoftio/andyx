using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Messages
{
    public class MessageAcknowledgedDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }

        public string AcknowledgedByConsumer { get; set; }
        public bool IsAcknowledged { get; set; }
        public Guid MessageId { get; set; }
    }
}
