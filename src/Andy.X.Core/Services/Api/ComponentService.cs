using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.IO.Readers;
using Buildersoft.Andy.X.IO.Writers;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Buildersoft.Andy.X.Core.Services.Api
{
    public class ComponentService : IComponentService
    {
        private readonly ILogger<ComponentService> _logger;
        private readonly ITenantRepository _tenantRepository;

        public ComponentService(ILogger<ComponentService> logger, ITenantRepository tenantRepository)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
        }

        public string AddComponentToken(string tenantName, string productName, string componentName, ComponentToken componentToken)
        {
            List<TenantConfiguration> tenants = TenantIOReader.ReadTenantsFromConfigFile();
            var tenant = tenants.Find(x => x.Name == tenantName);
            if (tenant == null)
                return null;

            var product = tenant.Products.Find(x => x.Name == productName);
            if (product == null)
                return null;

            var component = product.Components.Find(x => x.Name == componentName);
            if (component == null)
                return null;

            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            string apiKey = Convert.ToBase64String(key);
            componentToken.Token = apiKey;
            component.Settings.Tokens.Add(componentToken);

            // store token in memory!
            _tenantRepository.AddComponentToken(tenantName, productName, componentName, componentToken);

            // Write into file
            if (TenantIOWriter.WriteTenantsConfiguration(tenants) == true)
                return apiKey;

            return null;
        }

        public Component GetComponent(string tenantName, string productName, string componentName)
        {
            try
            {
                return _tenantRepository.GetComponent(tenantName, productName, componentName);
            }
            catch (Exception)
            {
                // TODO Log later
                return null;
            }
        }

        public List<Component> GetComponents(string tenantName, string productName)
        {
            var result = new List<Component>();
            try
            {
                var components = _tenantRepository.GetComponents(tenantName, productName);
                foreach (var component in components)
                {
                    result.Add(new Component() { Id = component.Value.Id, Name = component.Value.Name, Settings = component.Value.Settings });
                }
            }
            catch (Exception)
            {
                // TODO Log later
            }
            return result;
        }

        public List<ComponentToken> GetComponentTokens(string tenantName, string productName, string componentName)
        {
            return _tenantRepository.GetComponentTokens(tenantName, productName, componentName);
        }
    }
}
