using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using System.Collections.Generic;
using System.IO;

namespace Buildersoft.Andy.X.IO.Writers
{
    public static class TenantIOWriter
    {
        public static bool WriteTenantsConfiguration(List<TenantConfiguration> tenantConfigurations)
        {
            if (File.Exists(ConfigurationLocations.GetTenantsConfigurationFile()))
                File.Delete(ConfigurationLocations.GetTenantsConfigurationFile());

            try
            {
                File.WriteAllText(ConfigurationLocations.GetTenantsConfigurationFile(), tenantConfigurations.ToPrettyJson());
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static bool WriteStorageConfiguration(StorageConfiguration storage)
        {
            if (File.Exists(ConfigurationLocations.GetStorageConfigurationFile()))
                File.Delete(ConfigurationLocations.GetStorageConfigurationFile());

            try
            {
                File.WriteAllText(ConfigurationLocations.GetStorageConfigurationFile(), storage.ToPrettyJson());
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static bool WriteTransportConfiguration(TransportConfiguration transport)
        {
            if (File.Exists(ConfigurationLocations.GetTransportConfigurationFile()))
                File.Delete(ConfigurationLocations.GetTransportConfigurationFile());

            try
            {
                File.WriteAllText(ConfigurationLocations.GetTransportConfigurationFile(), transport.ToPrettyJson());
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }


        public static bool WriteClusterConfiguration(ClusterConfiguration cluster)
        {
            if (File.Exists(ConfigurationLocations.GetClustersConfigurationFile()))
                File.Delete(ConfigurationLocations.GetClustersConfigurationFile());

            try
            {
                File.WriteAllText(ConfigurationLocations.GetClustersConfigurationFile(), cluster.ToPrettyJson());
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
