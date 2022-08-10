using System;
using System.ComponentModel.DataAnnotations;

namespace Buildersoft.Andy.X.Model.Entities.Storages
{
    public class TopicEntryPosition
    {
        //default id is "_nodeConfiguration.NodeId"
        [Key]
        public string Id { get; set; }

        public long CurrentEntry { get; set; }
        public long MarkDeleteEntryPosition { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
