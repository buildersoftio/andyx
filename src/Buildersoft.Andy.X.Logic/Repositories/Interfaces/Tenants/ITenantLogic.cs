using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Tenants
{
    public interface ITenantLogic
    {
        Tenant CreateTenant(string tenantName);
        Tenant GetTenant(string tenantName);
        List<Tenant> GetAllTenants();
    }
}
