using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Utility.Extensions.Json;

namespace Buildersoft.Andy.X.Model.Entities.Core.Products
{
    public class ProductToken
    {
        [JsonIgnore]
        [ForeignKey("Products")]
        public long ProductId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string Secret { get; set; }

        public List<ProductTokenRole> Roles { get; set; }

        [JsonIgnore]
        [Column("Roles", TypeName = "json")]
        public string _Roles
        {
            get
            {
                return Roles.ToJson();
            }
            set
            {
                Roles = value.JsonToObject<List<ProductTokenRole>>();
            }
        }

        public bool IsActive { get; set; }
        public DateTimeOffset ExpireDate { get; set; }

        public string Description { get; set; }
        public DateTimeOffset IssuedDate { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }

    public enum ProductTokenRole
    {
        Produce,
        Consume
    }
}
