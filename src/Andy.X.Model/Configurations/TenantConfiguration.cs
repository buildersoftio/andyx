using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Tenants;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Configurations
{
    public class TenantConfiguration
    {
        public string Name { get; set; }
        public List<ProductConfiguration> Products { get; set; }
        public TenantSettings Settings { get; set; }

        public TenantConfiguration()
        {
            Products = new List<ProductConfiguration>();
            Settings = new TenantSettings();
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
        public ComponentSettings Settings { get; set; }

        public ComponentConfiguration()
        {
            Topics = new List<TopicConfiguration>();
            Settings = new ComponentSettings();
        }
    }

    public class TopicConfiguration
    {
        public string Name { get; set; }
    }
}
