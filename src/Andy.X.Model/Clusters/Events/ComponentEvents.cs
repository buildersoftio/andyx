using Buildersoft.Andy.X.Model.App.Components;
using System;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class ComponentCreatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Entities.Core.Components.ComponentSettings Settings { get; set; }
    }

    public class ComponentUpdatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public Entities.Core.Components.ComponentSettings Settings { get; set; }
    }

    public class ComponentDeletedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Name { get; set; }
    }
}
