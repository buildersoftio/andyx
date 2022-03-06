using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class ConfigurationDependencyInjectionExtensions
    {
        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.BindTenantsConfiguration(configuration);
            services.BindStorageConfiguration(configuration);
            services.BindCredentialsConfiguration(configuration);
        }

        private static void BindTenantsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var nodeConfiguration = new List<TenantConfiguration>();
            nodeConfiguration = JsonConvert.DeserializeObject<List<TenantConfiguration>>(File.ReadAllText(ConfigurationLocations.GetTenantsConfigurationFile()));
            services.AddSingleton(nodeConfiguration);
        }

        private static void BindStorageConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConfiguration = new StorageConfiguration();
            storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(File.ReadAllText(ConfigurationLocations.GetStorageConfigurationFile()));
            services.AddSingleton(storageConfiguration);
        }

        private static void BindCredentialsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var credentialsConfiguration = new CredentialsConfiguration();
            configuration.Bind("Credentials", credentialsConfiguration);
            services.AddSingleton(credentialsConfiguration);
        }
    }
}
