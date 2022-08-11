using Buildersoft.Andy.X.Model.App.Tenants;
using System;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class TenantCreatedArgs
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TenantSettings Settings { get; set; }
    }
    public class TenantUpdatedArgs
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TenantSettings Settings { get; set; }
    }

    public class TenantDeletedArgs
    {
        public string Name { get; set; }
    }
}
