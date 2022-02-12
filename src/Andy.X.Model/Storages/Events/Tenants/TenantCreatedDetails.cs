using Buildersoft.Andy.X.Model.App.Tenants;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Storages.Events.Tenants
{
    public class TenantCreatedDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TenantSettings Settings { get; set; }
        public List<string> StoragesAlreadySent { get; set; }

        public TenantCreatedDetails()
        {
            Settings = new TenantSettings();
            StoragesAlreadySent = new List<string>();
        }
    }
}
