using Buildersoft.Andy.X.Model.App.Components;
using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Components
{
    public class ComponentCreatedDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public ComponentSettings Settings { get; set; }
        public ComponentCreatedDetails()
        {
            Settings = new ComponentSettings();
        }
    }
}
