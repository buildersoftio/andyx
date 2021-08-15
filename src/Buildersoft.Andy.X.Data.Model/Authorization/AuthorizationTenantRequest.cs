using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.Authorization
{
    public class AuthorizationTenantRequest
    {
        public Guid TenantId { get; set; }
        public string SecurityKey { get; set; }
    }
}
