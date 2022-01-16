using Buildersoft.Andy.X.Model.App.Tenants;
using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Tenants
{
    public class TenantCreatedDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TenantSettings Settings { get; set; }
        public TenantCreatedDetails()
        {
            Settings = new TenantSettings();
        }
    }
}
