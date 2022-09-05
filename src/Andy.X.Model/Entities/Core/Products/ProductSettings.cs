using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Entities.Core.Products
{
    public class ProductSettings
    {
        [JsonIgnore]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [JsonIgnore]
        [ForeignKey("Products")]
        public long ProductId { get; set; }

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
