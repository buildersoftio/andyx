using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.IO.Writers;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class ConfigurationDependencyInjectionExtensions
    {
        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.BindNodeConfiguration(configuration);
            services.BindStorageConfiguration(configuration);
            services.BindCredentialsConfiguration(configuration);
            services.BindClusterConfiguration(configuration);
            services.BindTransportConfiguration(configuration);

            services.BindThreadsConfiguration(configuration);
        }

        private static void TryCreateCoreDirectories()
        {
            if (Directory.Exists(ConfigurationLocations.GetDataDirectory()) != true)
                Directory.CreateDirectory(ConfigurationLocations.GetDataDirectory());

            if (Directory.Exists(ConfigurationLocations.ActiveConfigDirectory()) != true)
                Directory.CreateDirectory(ConfigurationLocations.ActiveConfigDirectory());

            if (Directory.Exists(ConfigurationLocations.TempDirectory()) != true)
                Directory.CreateDirectory(ConfigurationLocations.TempDirectory());

            if (Directory.Exists(ConfigurationLocations.StorageDirectory()) != true)
                Directory.CreateDirectory(ConfigurationLocations.StorageDirectory());

            if (Directory.Exists(ConfigurationLocations.NodeLoggingDirectory()) != true)
                Directory.CreateDirectory(ConfigurationLocations.NodeLoggingDirectory());
        }

        private static void BindNodeConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // check if data directory exists.
            TryCreateCoreDirectories();

            var nodeConfiguration = new NodeConfiguration();

            nodeConfiguration.NodeId = configuration.GetValue<string>("NodeId");

            services.AddSingleton(nodeConfiguration);
        }

        private static void BindStorageConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConfiguration = new StorageConfiguration();

            if (File.Exists(ConfigurationLocations.GetStorageConfigurationFile()) != true)
            {
                storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(File.ReadAllText(ConfigurationLocations.GetStorageInitialConfigurationFile()));
                TenantIOWriter.WriteStorageConfiguration(storageConfiguration);
            }

            storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(File.ReadAllText(ConfigurationLocations.GetStorageConfigurationFile()));
            services.AddSingleton(storageConfiguration);
        }

        private static void BindClusterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var clusterConfiguration = new ClusterConfiguration();

            if (File.Exists(ConfigurationLocations.GetClustersConfigurationFile()) != true)
            {
                clusterConfiguration = JsonConvert.DeserializeObject<ClusterConfiguration>(File.ReadAllText(ConfigurationLocations.GetClusterInitialConfigurationFile()));
                TenantIOWriter.WriteClusterConfiguration(clusterConfiguration);
            }

            clusterConfiguration = JsonConvert.DeserializeObject<ClusterConfiguration>(File.ReadAllText(ConfigurationLocations.GetClustersConfigurationFile()));

            services.AddSingleton(clusterConfiguration);
        }

        private static void BindCredentialsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var credentialsConfiguration = new CredentialsConfiguration();
            configuration.Bind("Credentials", credentialsConfiguration);
            services.AddSingleton(credentialsConfiguration);
        }


        private static void BindThreadsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var agentConfiguration = new ThreadsConfiguration();
            configuration.Bind("Threads", agentConfiguration);
            services.AddSingleton(agentConfiguration);
        }

        private static void BindTransportConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var transportConfiguration = new TransportConfiguration();

            if (File.Exists(ConfigurationLocations.GetTransportConfigurationFile()) != true)
            {
                transportConfiguration = JsonConvert.DeserializeObject<TransportConfiguration>(File.ReadAllText(ConfigurationLocations.GetTransportInitialConfigurationFile()));
                TenantIOWriter.WriteTransportConfiguration(transportConfiguration);
            }

            transportConfiguration = JsonConvert.DeserializeObject<TransportConfiguration>(File.ReadAllText(ConfigurationLocations.GetTransportConfigurationFile()));
            services.AddSingleton(transportConfiguration);
        }

    }
}
