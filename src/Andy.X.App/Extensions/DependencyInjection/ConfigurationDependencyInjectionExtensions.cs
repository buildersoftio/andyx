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
        }

        private static void BindTenantsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var nodeConfiguration = new List<TenantConfiguration>();
            nodeConfiguration = JsonConvert.DeserializeObject<List<TenantConfiguration>>(File.ReadAllText(ConfigurationLocations.GetTenantsConfigurationFile()));
            services.AddSingleton(nodeConfiguration);
        }
    }
}
