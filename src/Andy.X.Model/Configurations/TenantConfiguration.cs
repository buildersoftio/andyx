using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.Subscriptions;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Configurations
{
    public class TenantConfiguration
    {
        public string Name { get; set; }
        public List<ProductConfiguration> Products { get; set; }
        public Entities.Core.Tenants.TenantSettings Settings { get; set; }

        public TenantConfiguration()
        {
            Products = new List<ProductConfiguration>();
            Settings = new Entities.Core.Tenants.TenantSettings();
        }
    }

    public class ProductConfiguration
    {
        public string Name { get; set; }
        public List<ComponentConfiguration> Components { get; set; }

        public ProductConfiguration()
        {
            Components = new List<ComponentConfiguration>();
        }
    }

    public class ComponentConfiguration
    {
        public string Name { get; set; }
        public List<TopicConfiguration> Topics { get; set; }
        public Entities.Core.Components.ComponentSettings Settings { get; set; }

        public ComponentConfiguration()
        {
            Topics = new List<TopicConfiguration>();
            Settings = new Entities.Core.Components.ComponentSettings();
        }
    }

    public class TopicConfiguration
    {
        public string Name { get; set; }

        // Key: SubscriptionName
        public Dictionary<string, SubscriptionConfiguration> Subscriptions { get; set; }
        public TopicConfiguration()
        {
            Subscriptions = new Dictionary<string, SubscriptionConfiguration>();
        }
    }

    public class SubscriptionConfiguration
    {
        // Name of the Subscription is the key in Directory
        //public string SubscriptionName { get; set; }

        public SubscriptionType SubscriptionType { get; set; }
        public SubscriptionMode SubscriptionMode { get; set; }
        public InitialPosition InitialPosition { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
    }
}
