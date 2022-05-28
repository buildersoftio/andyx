using MessagePack;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Consumers.Events
{
    [MessagePackObject]
    public class MessageSentDetails
    {
        [Key(0)]
        public string Tenant { get; set; }
        [Key(1)]
        public string Product { get; set; }
        [Key(2)]
        public string Component { get; set; }
        [Key(3)]
        public string Topic { get; set; }

        [Key(4)]
        public long LedgerId { get; set; }
        [Key(5)]
        public long EntryId { get; set; }

        [Key(6)]
        public Dictionary<string, string> Headers { get; set; }

        [Key(7)]
        public string MessageId { get; set; }

        [Key(8)]
        public byte[] Payload { get; set; }

        [Key(9)]
        public DateTimeOffset SentDate { get; set; }
    }
}
