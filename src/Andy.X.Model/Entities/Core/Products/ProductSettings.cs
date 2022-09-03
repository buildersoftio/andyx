using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildersoft.Andy.X.Model.Entities.Core.Products
{
    public class ProductSettings
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("Products")]
        public long ProductId { get; set; }

        public bool IsAuthorizationEnabled { get; set; }

        // internal settings
        public bool IsMarkedForDeletion { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        [Required]
        public string CreatedBy { get; set; }
    }
}
