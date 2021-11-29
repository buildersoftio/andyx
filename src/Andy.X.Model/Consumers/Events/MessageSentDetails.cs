using System;

namespace Buildersoft.Andy.X.Model.Consumers.Events
{
    public class MessageSentDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }

        public Guid Id { get; set; }
        public object MessageRaw { get; set; }

        public DateTime SentDate { get; set; }
    }
}
