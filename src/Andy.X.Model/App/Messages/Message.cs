using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.App.Messages
{
    [ProtoContract]
    public class Message
    {
        [ProtoMember(1)]
        public string Tenant { get; set; }
        [ProtoMember(2)]
        public string Product { get; set; }
        [ProtoMember(3)]
        public string Component { get; set; }
        [ProtoMember(4)]
        public string Topic { get; set; }

        [ProtoMember(5)]
        public Dictionary<string, object> Headers { get; set; }

        [ProtoMember(6)]
        public string Id { get; set; }
        [ProtoMember(7)]
        public object MessageRaw { get; set; }

        [ProtoMember(8)]
        public DateTime SentDate { get; set; }

        public Message()
        {
            // if this property is not coming from Andy X Client
            SentDate = DateTime.UtcNow;
            Headers = new Dictionary<string, object>();
        }
    }
}
