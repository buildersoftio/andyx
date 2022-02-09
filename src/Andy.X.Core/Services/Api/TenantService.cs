﻿using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.IO.Readers;
using Buildersoft.Andy.X.IO.Writers;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

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

            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            string apiKey = Convert.ToBase64String(key);

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
                return apiKey;

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
                tenant.Settings.Tokens.Clear();
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

        public TenantConfiguration UpdateTenantSettings(string tenantName, TenantSettings tenantSettings)
        {
            throw new System.NotImplementedException();
        }
    }
}
