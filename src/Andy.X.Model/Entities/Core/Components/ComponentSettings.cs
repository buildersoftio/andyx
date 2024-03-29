﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Entities.Core.Components
{
    public class ComponentSettings
    {
        [JsonIgnore]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [JsonIgnore]
        [ForeignKey("Components")]
        public long ComponentId { get; set; }

        public bool IsTopicAutomaticCreationAllowed { get; set; }
        public bool EnforceSchemaValidation { get; set; }
        public bool IsAuthorizationEnabled { get; set; }

        public bool IsSubscriptionAutomaticCreationAllowed { get; set; }
        public bool IsProducerAutomaticCreationAllowed { get; set; }

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
