using System;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class ComponentCreatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Name { get; set; }
        public Entities.Core.Components.Component Component{ get; set; }
        public Entities.Core.Components.ComponentSettings Settings { get; set; }
    }
    public class ComponentUpdatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }

        public string Component { get; set; }

        public Entities.Core.Components.ComponentSettings Settings { get; set; }
    }
    public class ComponentDeletedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Name { get; set; }
    }

    public class ComponentTokenCreatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public Entities.Core.Components.ComponentToken ComponentToken { get; set; }
    }
    public class ComponentTokenRevokedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public Guid Key { get; set; }
    }
    public class ComponentTokenDeletedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public Guid Key { get; set; }
    }

    public class ComponentRetentionCreatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public Entities.Core.Components.ComponentRetention ComponentRetention { get; set; }
    }
    public class ComponentRetentionUpdatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public Entities.Core.Components.ComponentRetention ComponentRetention { get; set; }
    }
    public class ComponentRetentionDeletedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public Entities.Core.Components.ComponentRetention ComponentRetention { get; set; }
    }
}
