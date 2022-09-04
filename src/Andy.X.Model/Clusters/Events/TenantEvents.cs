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
}
