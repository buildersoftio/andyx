using MessagePack;
using System.Collections.Generic;
using System;
using ProtoBuf;

namespace Buildersoft.Andy.X.Model.Entities.Clusters
{
    [ProtoContract]
    [MessagePackObject]
    [Serializable]
    public class ClusterChangeLog
    {
        [ProtoMember(1)]
        [Key(0)]
        public long Entry { get; set; }

        [ProtoMember(2)]
        [Key(1)]
        public string Tenant { get; set; }

        [ProtoMember(3)]
        [Key(2)]
        public string Product { get; set; }

        [ProtoMember(4)]
        [Key(3)]
        public string Component { get; set; }

        [ProtoMember(5)]
        [Key(4)]
        public string Topic { get; set; }

        [ProtoMember(6)]
        [Key(5)]
        public Dictionary<string, string> Headers { get; set; }

        [ProtoMember(7)]
        [Key(6)]
        public byte[] Id { get; set; }
        [ProtoMember(8)]

        [Key(7)]
        public byte[] Payload { get; set; }

        [ProtoMember(9)]
        [Key(8)]
        public DateTimeOffset SentDate { get; set; }

        public ClusterChangeLog()
        {
            // if this property is not coming from Andy X Client
            SentDate = DateTimeOffset.UtcNow;
            Headers = new Dictionary<string, string>();
        }
    }
}
