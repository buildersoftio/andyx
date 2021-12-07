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

    public class TenantSettings
    {
        public bool AllowProductCreation { get; set; }
        public string DigitalSignature { get; set; }
        public bool EnableEncryption { get; set; }

        // Split tenants by certificates will not be possible with version two
        public string CertificatePath { get; set; }

        public TenantSettings()
        {
            AllowProductCreation = true;
            EnableEncryption = false;
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

    public class ComponentSettings
    {
        public bool AllowSchemaValidation { get; set; }
        public bool AllowTopicCreation { get; set; }

        public ComponentSettings()
        {
            AllowSchemaValidation = false;
            AllowTopicCreation = true;
        }
    }

    public class TopicConfiguration
    {
        public string Name { get; set; }
    }
}
