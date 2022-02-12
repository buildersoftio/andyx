using Buildersoft.Andy.X.Model.App.Tenants;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Storages.Requests.Tenants
{
    public class CreateTenantDetails
    {
        public string Name { get; set; }
        public TenantSettings TenantSettings { get; set; }

        public List<string> StoragesAlreadySent { get; set; }
        public CreateTenantDetails()
        {
            TenantSettings = new TenantSettings();
            StoragesAlreadySent = new List<string>();
        }
    }
}
