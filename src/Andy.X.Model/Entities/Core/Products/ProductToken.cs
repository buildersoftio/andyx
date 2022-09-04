using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Buildersoft.Andy.X.Model.Entities.Core.Products
{
    public class ProductToken
    {
        [ForeignKey("Products")]
        public long ProductId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Secret { get; set; }

        [Column(TypeName = "json")]
        public string Roles { get; set; }

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

    public enum ProductTokenRole
    {
        Produce,
        Consume
    }
}
