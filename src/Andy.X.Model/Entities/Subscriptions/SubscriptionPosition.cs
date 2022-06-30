using System;
using System.ComponentModel.DataAnnotations;

namespace Buildersoft.Andy.X.Model.Entities.Subscriptions
{
    public class SubscriptionPosition
    {
        [Key]
        public string SubscriptionName { get; set; }

        public long MarkDeleteEntryPosition { get; set; }  

        // this is the most important property
        public long ReadEntryPosition { get; set; }

        public bool IsConnected { get; set; }

        public long PendingReadOperations { get; set; }
        public long EntriesSinceLastUnacked { get; set; }

        public DateTimeOffset? UpdatedDate{ get; set; }
        public DateTimeOffset CreatedDate{ get; set; }
    }
}
