using System;
using System.IO;

namespace Buildersoft.Andy.X.IO.Locations
{
    public static class ConfigurationLocations
    {
        public static string GetRootDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string ConfigDirectory()
        {
            return Path.Combine(GetRootDirectory(), "config");
        }

        public static string GetTenantsConfigurationFile()
        {
            return Path.Combine(ConfigDirectory(), "tenants.json");
        }

        public static string GetStorageConfigurationFile()
        {
            return Path.Combine(ConfigDirectory(), "storage_config.json");
        }

        public static string GetUsersConfigurationFile()
        {
            return Path.Combine(ConfigDirectory(), "users_config.json");
        }

        public static string GetClustersConfigurationFile()
        {
            return Path.Combine(ConfigDirectory(), "clusters_config.json");
        }
    }
}
