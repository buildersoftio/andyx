using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.IO.Readers;
using Buildersoft.Andy.X.IO.Writers;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Utility.Generators;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Buildersoft.Andy.X.Core.Services.Api
{
    public class TenantService : ITenantService
    {
        private readonly ILogger<TenantService> _logger;
        private readonly ITenantRepository _tenantRepository;
        private readonly IStorageHubService _storageHubService;
        private readonly ITenantFactory _tenantFactory;

        public TenantService(ILogger<TenantService> logger, ITenantRepository tenantRepository, IStorageHubService storageHubService, ITenantFactory tenantFactory)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _storageHubService = storageHubService;
            _tenantFactory = tenantFactory;
        }

        public string AddToken(string tenantName, DateTime expireDate)
        {
            List<TenantConfiguration> tenants = TenantIOReader.ReadTenantsFromConfigFile();
            var tenant = tenants.Find(x => x.Name == tenantName);
            if (tenant == null)
                return null;

            string apiKey = KeyGenerators.GenerateApiKey();

            var tenantToken = new TenantToken()
            {
                ExpireDate = expireDate,
                Token = apiKey,
                IsActive = true,
                IssuedDate = DateTime.Now,
                IssuedFor = $"Issued for tenant {tenantName}"
            };

            tenant.Settings.Tokens.Add(tenantToken);
            _tenantRepository.AddTenantToken(tenantName, tenantToken);

            // Write into file
            if (TenantIOWriter.WriteTenantsConfiguration(tenants) == true)
            {
                // Send to the Cluster
                _storageHubService.SendCreateTenantTokenStorage(new Model.Storages.Requests.Tenants.CreateTenantTokenDetails()
                {
                    Tenant = tenantName,
                    Token = tenantToken,
                    StoragesAlreadySent = new List<string>()
                });

                return apiKey;
            }

            return null;
        }

        public string AddToken(string tenantName, TenantToken tenantToken)
        {
            List<TenantConfiguration> tenants = TenantIOReader.ReadTenantsFromConfigFile();
            var tenant = tenants.Find(x => x.Name == tenantName);
            if (tenant == null)
                return null;

            tenant.Settings.Tokens.Add(tenantToken);
            _tenantRepository.AddTenantToken(tenantName, tenantToken);

            // Write into file
            if (TenantIOWriter.WriteTenantsConfiguration(tenants) == true)
                return tenantToken.Token;

            return null;
        }

        public bool CreateTenant(string tenantName, TenantSettings tenantSettings)
        {
            List<TenantConfiguration> tenants = TenantIOReader.ReadTenantsFromConfigFile();
            var tenant = tenants.Find(x => x.Name == tenantName);
            if (tenant != null)
                return false;

            var tenantConfiguration = new TenantConfiguration() { Name = tenantName, Settings = tenantSettings };
            tenants.Add(tenantConfiguration);
            if (TenantIOWriter.WriteTenantsConfiguration(tenants) == true)
            {
                _tenantRepository.AddTenantFromApi(tenantConfiguration);
                _storageHubService.CreateTenantAsync(_tenantFactory
                   .CreateTenant(tenantConfiguration.Name,
                       tenantConfiguration.Settings.DigitalSignature,
                       tenantConfiguration.Settings.EnableEncryption,
                       tenantConfiguration.Settings.AllowProductCreation,
                       tenantConfiguration.Settings.EnableAuthorization,
                       tenantConfiguration.Settings.Tokens,
                       tenantConfiguration.Settings.Logging,
                       tenantConfiguration.Settings.EnableGeoReplication,
                       tenantConfiguration.Settings.CertificatePath));
                return true;
            }

            return false;
        }

        public TenantConfiguration GetTenant(string tenantName)
        {
            List<TenantConfiguration> tenants = TenantIOReader.ReadTenantsFromConfigFile();
            var tenant = tenants.Find(x => x.Name == tenantName);
            if (tenant != null)
            {
                tenant.Products.Clear();
            }
            return tenant;
        }

        public List<string> GetTenantsName()
        {
            List<TenantConfiguration> tenants = TenantIOReader.ReadTenantsFromConfigFile();
            return tenants.Select(x => x.Name).ToList();
        }

        public List<TenantToken> GetTokens(string tenantName)
        {
            List<TenantConfiguration> tenants = TenantIOReader.ReadTenantsFromConfigFile();
            var tenant = tenants.Find(x => x.Name == tenantName);
            if (tenant == null)
                return null;

            return tenant.Settings.Tokens;
        }

        public bool RevokeToken(string tenantName, string token)
        {
            // Remove from the memory
            _tenantRepository.RemoveTenantToken(tenantName, token);
            // remove from config file

            List<TenantConfiguration> tenants = TenantIOReader.ReadTenantsFromConfigFile();
            var tenant = tenants.Find(x => x.Name == tenantName);
            if (tenant == null)
                return false;

            // remove from config file.
            var tenantToken = tenant.Settings.Tokens.Find(x => x.Token == token);
            if (tenantToken == null)
                return true;

            tenant.Settings.Tokens.Remove(tenantToken);

            // Write into file
            if (TenantIOWriter.WriteTenantsConfiguration(tenants) == true)
            {
                // Send to the Cluster
                _storageHubService.SendRevokeTenantTokenStorage(new Model.Storages.Requests.Tenants.RevokeTenantTokenDetails()
                {
                    Tenant = tenantName,
                    Token = token,
                    StoragesAlreadySent = new List<string>()
                });
                return true;
            }

            return false;
        }

        public TenantConfiguration UpdateTenantSettings(string tenantName, TenantSettings tenantSettings)
        {
            throw new System.NotImplementedException();
        }
    }
}
