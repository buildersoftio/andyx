using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Storages.Events.Tenants
{
    public class TenantTokenRevokedDetails
    {
        public string Tenant { get; set; }
        public string Token { get; set; }

        public List<string> StoragesAlreadySent { get; set; }


        public TenantTokenRevokedDetails()
        {
            StoragesAlreadySent = new List<string>();
        }
    }
}
