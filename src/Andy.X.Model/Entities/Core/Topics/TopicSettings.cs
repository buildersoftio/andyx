using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Entities.Core.Topics
{
    public class TopicSettings
    {
        // Topic Storage Settings
        [JsonIgnore]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [JsonIgnore]
        [ForeignKey("Topics")]
        public long TopicId { get; set; }

        public ulong WriteBufferSizeInBytes { get; set; }
        public int MaxWriteBufferNumber { get; set; }

        public int MaxWriteBufferSizeToMaintain { get; set; }
        public int MinWriteBufferNumberToMerge { get; set; }
        public int MaxBackgroundCompactionsThreads { get; set; }
        public int MaxBackgroundFlushesThreads { get; set; }


        [JsonIgnore]
        public DateTimeOffset? UpdatedDate { get; set; }
        [JsonIgnore]
        public DateTimeOffset CreatedDate { get; set; }

        [JsonIgnore]
        public string UpdatedBy { get; set; }

        [JsonIgnore]
        public string CreatedBy { get; set; }
    }
}
