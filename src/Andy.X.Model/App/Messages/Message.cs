﻿using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.App.Messages
{
    [ProtoContract]
    [MessagePackObject]
    [Serializable]
    public class Message
    {
        [ProtoMember(1)]
        [Key(0)]
        public string Tenant { get; set; }
        [ProtoMember(2)]
        [Key(1)]
        public string Product { get; set; }
        [ProtoMember(3)]
        [Key(2)]
        public string Component { get; set; }
        [ProtoMember(4)]
        [Key(3)]
        public string Topic { get; set; }

        [ProtoMember(5)]
        [Key(4)]
        public Dictionary<string, string> Headers { get; set; }

        [ProtoMember(6)]
        [Key(5)]
        public byte[] Id { get; set; }
        [ProtoMember(7)]
        [Key(6)]
        public byte[] Payload { get; set; }

        [ProtoMember(8)]
        [Key(7)]
        public DateTimeOffset SentDate { get; set; }

        [ProtoMember(9)]
        [Key(8)]
        public Guid IdentityId { get; set; }

        [ProtoMember(10)]
        [Key(9)]
        public string NodeId { get; set; }

        [ProtoMember(11)]
        [Key(10)]
        public bool RequiresCallback { get; set; }

        public Message()
        {
            // if this property is not coming from Andy X Client
            SentDate = DateTimeOffset.UtcNow;
            Headers = new Dictionary<string, string>();
            NodeId = "";

            RequiresCallback = false;
        }
    }
}
