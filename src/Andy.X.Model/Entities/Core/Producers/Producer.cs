using Buildersoft.Andy.X.Utility.Extensions.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Entities.Core.Producers
{
    public class Producer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [JsonIgnore]
        [ForeignKey("Topics")]
        public long TopicId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public ProducerInstanceType InstanceType { get; set; }
        
        public List<string> PublicIpRange { get; set; }
        public List<string> PrivateIpRange { get; set; }

        [JsonIgnore]
        [Column("PublicIpRange", TypeName = "json")]
        public string _PublicIpRange
        {
            get
            {
                return PublicIpRange.ToJson();
            }
            set
            {
                PublicIpRange = value.JsonToObject<List<string>>();
            }
        }

        [JsonIgnore]
        [Column("PrivateIpRange", TypeName = "json")]
        public string _PrivateIpRange
        {
            get
            {
                return PrivateIpRange.ToJson();
            }
            set
            {
                PrivateIpRange = value.JsonToObject<List<string>>();
            }
        }

        [JsonIgnore]
        public bool IsMarkedForDeletion { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }

    public enum ProducerInstanceType
    {
        Single,
        Multiple
    }
}
