using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Storages.Requests.Tenants
{
    public class RevokeTenantTokenDetails
    {
        public string Tenant { get; set; }
        public string Token { get; set; }

        public List<string> StoragesAlreadySent { get; set; }

        public RevokeTenantTokenDetails()
        {
            StoragesAlreadySent = new List<string>();
        }
    }
}
