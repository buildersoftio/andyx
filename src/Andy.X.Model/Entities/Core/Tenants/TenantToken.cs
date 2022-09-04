using Buildersoft.Andy.X.Utility.Extensions.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Entities.Core.Tenants
{
    public class TenantToken
    {
        [JsonIgnore]
        [ForeignKey("Tenants")]
        public long TenantId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string Secret { get; set; }

        public bool IsActive { get; set; }


        public List<TenantTokenRole> Roles { get; set; }

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
                Roles = value.JsonToObject<List<TenantTokenRole>>();
            }
        }

        public DateTimeOffset ExpireDate { get; set; }

        public string Description { get; set; }
        public DateTimeOffset IssuedDate { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public string CreatedBy { get; set; }
    }

    public enum TenantTokenRole
    {
        Produce,
        Consume
    }

}
