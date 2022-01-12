using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Tenants;
using System;
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
        public bool EnableGeoReplication { get; set; }
        public TenantLogging Logging { get; set; }

        public bool EnableAuthorization { get; set; }
        public List<TenantToken> Tokens { get; set; }

        // Split tenants by certificates will not be possible with version two
        public string CertificatePath { get; set; }

        public TenantSettings()
        {
            AllowProductCreation = true;
            EnableEncryption = false;
            EnableAuthorization = false;
            Tokens = new List<TenantToken>();
        }
    }

    public class TenantToken
    {
        public string Token { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpireDate { get; set; }
        public string IssuedFor { get; set; }
        public DateTime IssuedDate { get; set; }
            EnableGeoReplication = false;
            Logging = TenantLogging.ERROR_ONLY;
        }
    }

    public enum TenantLogging
    {
        ALL,
        INFORMATION_ONLY,
        WARNING_ONLY,
        ERROR_ONLY,
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

        public bool EnableAuthorization { get; set; }
        public List<ComponentToken> Tokens { get; set; }


        public ComponentSettings()
        {
            AllowSchemaValidation = false;
            AllowTopicCreation = true;
            EnableAuthorization = false;

            Tokens = new List<ComponentToken>();
        }
    }


    public class TopicConfiguration
    {
        public string Name { get; set; }
    }
}
