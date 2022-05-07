using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.IO.Readers;
using Buildersoft.Andy.X.IO.Writers;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Utility.Generators;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Services.Api
{
    public class ComponentService : IComponentService
    {
        private readonly ILogger<ComponentService> _logger;
        private readonly ITenantRepository _tenantRepository;
        private readonly IStorageHubService _storageHubService;
        private readonly ITenantFactory _tenantFactory;

        public ComponentService(ILogger<ComponentService> logger, ITenantRepository tenantRepository, IStorageHubService storageHubService, ITenantFactory tenantFactory)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _storageHubService = storageHubService;
            _tenantFactory = tenantFactory;
        }
        public string AddComponentToken(string tenantName, string productName, string componentName, ComponentToken componentToken, bool shoudGenerateToken = true)
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

            if (shoudGenerateToken == true)
            {
                string apiKey = KeyGenerators.GenerateApiKey();
                componentToken.Token = apiKey;
            }

            component.Settings.Tokens.Add(componentToken);

            // store token in memory!
            _tenantRepository.AddComponentToken(tenantName, productName, componentName, componentToken);

            // Write into file
            if (TenantIOWriter.WriteTenantsConfiguration(tenants) == true)
            {
                if (shoudGenerateToken == true)
                {
                    // Send to the Cluster
                    _storageHubService.SendCreateComponentTokenStorage(new Model.Storages.Requests.Components.CreateComponentTokenDetails()
                    {
                        Tenant = tenantName,
                        Product = productName,
                        Component = componentName,
                        Token = componentToken,

                        StoragesAlreadySent = new List<string>()
                    });
                }
                return componentToken.Token;
            }

            return null;
        }

        public string AddRetentionPolicy(string tenantName, string productName, string componentName, ComponentRetention retention)
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


            component.Settings.RetentionPolicy = retention;
            // store token in memory!
            _tenantRepository.AddComponentRetention(tenantName, productName, componentName, retention);

            // Write into file
            if (TenantIOWriter.WriteTenantsConfiguration(tenants) == true)
                return retention.Name;

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

        public ComponentRetention GetRetentionPolicy(string tenantName, string productName, string componentName)
        {
            return _tenantRepository.GetComponentRetention(tenantName, productName, componentName);
        }

        public bool RevokeComponentToken(string tenantName, string productName, string componentName, string token)
        {
            // Remove from the memory
            _tenantRepository.RemoveComponentToken(tenantName, productName, componentName, token);
            // remove from config file

            List<TenantConfiguration> tenants = TenantIOReader.ReadTenantsFromConfigFile();
            var tenant = tenants.Find(x => x.Name == tenantName);
            if (tenant == null)
                return false;

            var product = tenant.Products.Find(x => x.Name == productName);
            if (product == null)
                return false;

            var component = product.Components.Find(x => x.Name == componentName);
            if (component == null)
                return false;


            var componentToken = component.Settings.Tokens.Find(x => x.Token == token);
            if (componentToken == null)
                return true;

            component.Settings.Tokens.Remove(componentToken);

            // write into file
            if (TenantIOWriter.WriteTenantsConfiguration(tenants) == true)
            {
                // Send to the Cluster
                _storageHubService.SendRevokeComponentTokenStorage(new Model.Storages.Requests.Components.RevokeComponentTokenDetails()
                {
                    Tenant = tenantName,
                    Product = productName,
                    Component = componentName,
                    Token = token,

                    StoragesAlreadySent = new List<string>()
                });

                return true;
            }

            return false;
        }

        public bool AddComponent(string tenantName, string productName, string componentName, ComponentSettings componentSettings)
        {
            return _tenantRepository
                .AddComponent(tenantName,
                    productName,
                    componentName,
                    _tenantFactory
                        .CreateComponent(componentName,
                            componentSettings.AllowSchemaValidation,
                            componentSettings.AllowTopicCreation,
                            componentSettings.EnableAuthorization,
                            tokens: new List<ComponentToken>()));
        }
    }
}
