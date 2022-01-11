using System;

namespace Buildersoft.Andy.X.Model.App.Tenants
{
    public class TenantToken
    {
        public string Token { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpireDate { get; set; }
        public string IssuedFor { get; set; }
        public DateTime IssuedDate { get; set; }
    }
}
