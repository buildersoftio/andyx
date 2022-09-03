using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Buildersoft.Andy.X.Model.Entities.Core.Components
{
    public class ComponentRetention
    {
        [ForeignKey("Components")]
        public long ComponentId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }
        public RetentionType Type { get; set; }
        public long TimeToLiveInMinutes { get; set; }
        

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        [Required]
        public string CreatedBy { get; set; }

    }
}
