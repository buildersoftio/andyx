using Buildersoft.Andy.X.Model.App.Tenants;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Storages.Events.Tenants
{
    public class TenantTokenCreatedDetails
    {
        public string Tenant { get; set; }
        public TenantToken Token { get; set; }

        public List<string> StoragesAlreadySent { get; set; }

        public TenantTokenCreatedDetails()
        {
            Token = new TenantToken();
            StoragesAlreadySent = new List<string>();
        }
    }
}
