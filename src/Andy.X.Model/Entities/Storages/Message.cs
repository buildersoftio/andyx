using MessagePack;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Entities.Storages
{
    [MessagePackObject]
    public class Message
    {
        [Key(0)]
        public long Entry { get; set; }

        [Key(1)]
        public byte[] MessageId { get; set; }

        [Key(2)]
        public Dictionary<string, string> Headers { get; set; }

        [Key(3)]
        public byte[] Payload { get; set; }


        // this property is  for clustering, to know to which Node to send the acknowledgment of the message.
        [Key(4)]
        public string NodeId { get; set; }


        [Key(5)]
        public DateTimeOffset StoredDate { get; set; }

        [Key(6)]
        public DateTimeOffset SentDate { get; set; }
    }
}
