using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Entities.Core.Tenants
{
    public class TenantSettings
    {
        [JsonIgnore]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [JsonIgnore]
        [ForeignKey("Tenants")]
        public long TenantId { get; set; }

        public bool IsProductAutomaticCreation { get; set; }
        public bool IsEncryptionEnabled { get; set; }
        public bool IsAuthorizationEnabled { get; set; }

        // internal settings
        [JsonIgnore]
        public bool IsMarkedForDeletion { get; set; }


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
