using Buildersoft.Andy.X.Model.App.Tenants;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Storages.Requests.Tenants
{
    public class CreateTenantTokenDetails
    {
        public string Tenant { get; set; }
        public TenantToken Token { get; set; }

        public List<string> StoragesAlreadySent { get; set; }

        public CreateTenantTokenDetails()
        {
            StoragesAlreadySent = new List<string>();
            Token = new TenantToken();
        }
    }
}
