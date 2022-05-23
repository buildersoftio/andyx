using MessagePack;
using System;

namespace Buildersoft.Andy.X.Model.Producers.Events
{
    [MessagePackObject]
    public class ProducerDisconnectedDetails
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
        public Guid Id { get; set; }
        [Key(5)]
        public string ProducerName { get; set; }
    }
}
