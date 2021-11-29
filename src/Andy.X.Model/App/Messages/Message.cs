using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.App.Messages
{
    public class Message
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }
        public List<string> ConsumersCurrentTransmitted { get; set; }

        public Guid Id { get; set; }
        public object MessageRaw { get; set; }

        public DateTime SentDate { get; set; }

        public Message()
        {
            // if this property is not coming from Andy X Client
            SentDate = DateTime.UtcNow;
        }
    }
}
