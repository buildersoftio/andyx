using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Buildersoft.Andy.X.Core.Services.Api
{
    public class TenantService : ITenantService
    {
        private readonly ILogger<TenantService> _logger;
        private readonly ITenantRepository _tenantRepository;

        public TenantService(ILogger<TenantService> logger, ITenantRepository tenantRepository)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
        }
        public bool CreateTenant(string tenantName, TenantSettings tenantSettings)
        {
            throw new System.NotImplementedException();
        }

        public TenantConfiguration GetTenant(string tenantName)
        {
            List<TenantConfiguration> tenants = File.ReadAllText(ConfigurationLocations.GetTenantsConfigurationFile()).JsonToObject<List<TenantConfiguration>>();
            var tenant = tenants.Find(x => x.Name == tenantName);
            if (tenant != null)
            {
                tenant.Products.Clear();
                tenant.Settings.Tokens.Clear();
            }
            return tenant;
        }

        public List<string> GetTenantsName()
        {
            List<TenantConfiguration> tenants = File.ReadAllText(ConfigurationLocations.GetTenantsConfigurationFile()).JsonToObject<List<TenantConfiguration>>();
            return tenants.Select(x => x.Name).ToList();
        }

        public TenantConfiguration UpdateTenantSettings(string tenantName, TenantSettings tenantSettings)
        {
            throw new System.NotImplementedException();
        }
    }
}
