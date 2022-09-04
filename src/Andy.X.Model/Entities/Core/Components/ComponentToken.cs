using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Entities.Core.Components
{
    public class ComponentToken
    {
        [JsonIgnore]
        [ForeignKey("Components")]
        public long ComponentId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string Secret { get; set; }

        public List<ComponentTokenRole> Roles { get; set; }

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
                Roles = value.JsonToObject<List<ComponentTokenRole>>();
            }
        }

        public bool IsActive { get; set; }
        public DateTimeOffset ExpireDate { get; set; }

        public string IssuedFor { get; set; }
        public string Desciption { get; set; }
        public DateTimeOffset IssuedDate { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        [Required]
        public string CreatedBy { get; set; }
    }

    public enum ComponentTokenRole
    {
        Produce,
        Consume
    }
}
