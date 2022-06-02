using MessagePack;
using System;

namespace Buildersoft.Andy.X.Model.App.Messages
{
    [MessagePackObject]
    public class MessageAcknowledgementFileContent
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
        public string Subscription { get; set; }

        [Key(5)]
        public long LedgerId { get; set; }
        [Key(6)]
        public long EntryId { get; set; }

        [Key(7)]
        public bool IsDeleted { get; set; }


        [Key(8)]
        public DateTimeOffset CreatedDate { get; set; }

    }
}
