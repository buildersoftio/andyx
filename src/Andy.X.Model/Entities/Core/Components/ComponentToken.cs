using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Buildersoft.Andy.X.Model.Entities.Core.Components
{
    public class ComponentToken
    {
        [ForeignKey("Components")]
        public long ComponentId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Secret { get; set; }

        public bool IsActive { get; set; }
        public DateTimeOffset ExpireDate { get; set; }

        public string Desciption { get; set; }
        public DateTimeOffset IssuedDate { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        [Required]
        public string CreatedBy { get; set; }
    }
}
