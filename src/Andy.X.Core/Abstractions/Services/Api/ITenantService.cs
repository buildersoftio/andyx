using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.Configurations;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Api
{
    public interface ITenantService
    {
        List<string> GetTenantsName();
        TenantConfiguration GetTenant(string tenantName);
        bool CreateTenant(string tenantName, TenantSettings tenantSettings);
        TenantConfiguration UpdateTenantSettings(string tenantName, TenantSettings tenantSettings);

        string AddToken(string tenantName, DateTime expireDate);
        List<TenantToken> GetTokens(string tenantName);
    }
}
