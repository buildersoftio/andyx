using System;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class TenantCreatedArgs
    {
        public string Name { get; set; }
        public Entities.Core.Tenants.TenantSettings Settings { get; set; }
    }
    public class TenantUpdatedArgs
    {
        public string Name { get; set; }
        public Entities.Core.Tenants.TenantSettings Settings { get; set; }
    }
    public class TenantDeletedArgs
    {
        public string Name { get; set; }
    }

    public class TenantTokenCreatedArgs
    {
        public string Tenant { get; set; }
        public Entities.Core.Tenants.TenantToken TenantToken { get; set; }
    }
    public class TenantTokenRevokedArgs
    {
        public string Tenant { get; set; }
        public Guid Key { get; set; }
    }
    public class TenantTokenDeletedArgs
    {
        public string Tenant { get; set; }
        public Guid Key { get; set; }
    }

    public class TenantRetentionCreatedArgs
    {
        public string Tenant { get; set; }
        public Entities.Core.Tenants.TenantRetention TenantRetention { get; set; }
    }
    public class TenantRetentionUpdatedArgs
    {
        public string Tenant { get; set; }
        public Entities.Core.Tenants.TenantRetention TenantRetention { get; set; }
    }
    public class TenantRetentionDeletedArgs
    {
        public string Tenant { get; set; }
        public Entities.Core.Tenants.TenantRetention TenantRetention { get; set; }
    }
}
