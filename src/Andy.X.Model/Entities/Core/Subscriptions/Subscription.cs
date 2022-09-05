﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Buildersoft.Andy.X.Model.Subscriptions;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Entities.Core.Subscriptions
{
    public class Subscription
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [JsonIgnore]
        [ForeignKey("Topics")]
        public long TopicId { get; set; }

        public string Name { get; set; }

        public SubscriptionType SubscriptionType { get; set; }
        public SubscriptionMode SubscriptionMode { get; set; }
        public InitialPosition InitialPosition { get; set; }


        // internal settings
        [JsonIgnore]
        public bool IsMarkedForDeletion { get; set; }


        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}
