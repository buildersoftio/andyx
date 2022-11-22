using MessagePack;
using System;

namespace Buildersoft.Andy.X.Model.Producers.Events
{
    [MessagePackObject]
    public class MessageAcceptedDetails
    {
        [Key(0)]
        public Guid IdentityId { get; set; }

        [Key(1)]
        public int MessageCount { get; set; }

        [Key(2)]
        public DateTimeOffset AcceptedDate { get; set; }
    }
}
