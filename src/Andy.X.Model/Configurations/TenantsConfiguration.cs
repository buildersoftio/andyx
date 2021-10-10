using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Configurations
{
    public class TenantsConfiguration
    {
        public List<Tenant> Tenants { get; set; }
    }

    public class Tenant
    {
        public string Name { get; set; }
        public TenantSettings Settings { get; set; }
    }

    public class TenantSettings
    {
        public bool AllowAutoTopicCreation { get; set; }
        public bool AllowSchemaValidation { get; set; }
        public string DigitalSignature { get; set; }

        public TenantSettings()
        {
            AllowAutoTopicCreation = true;
            AllowSchemaValidation = false;
        }
    }
}
