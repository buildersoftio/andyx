using Buildersoft.Andy.X.Model.App.Tenants;
using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Tenants
{
    public class TenantUpdatedDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TenantSettings Settings { get; set; }

        public TenantUpdatedDetails()
        {
            Settings = new TenantSettings();
        }
    }
}
