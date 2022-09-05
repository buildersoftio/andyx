using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Entities.Core.Topics
{
    public class Topic
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [JsonIgnore]
        [ForeignKey("Components")]
        public long ComponentId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        // internal settings
        [JsonIgnore]
        public bool IsMarkedForDeletion { get; set; }


        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}
