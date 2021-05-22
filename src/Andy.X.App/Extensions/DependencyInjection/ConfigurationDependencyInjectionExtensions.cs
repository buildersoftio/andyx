using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

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
            configuration.Bind("Tenants", nodeConfiguration);
            services.AddSingleton(nodeConfiguration);
        }
    }
}
