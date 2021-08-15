using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Components
{
    public class ComponentUpdatedDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
