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

        public static string GetDataDirectory()
        {
            return Path.Combine(GetRootDirectory(), "data");
        }

        public static string ActiveConfigDirectory()
        {
            return Path.Combine(GetDataDirectory(), "active");
        }

        public static string TempDirectory()
        {
            return Path.Combine(GetDataDirectory(), "temp");
        }

        public static string GetTempClusterRootDirectory()
        {
            return Path.Combine(TempDirectory(), "clusters");
        }


        public static string StorageDirectory()
        {
            return Path.Combine(GetDataDirectory(), "store");
        }

        public static string SettingsDirectory()
        {
            return Path.Combine(GetRootDirectory(), "settings");
        }

        public static string Settings_InitialConfigDirectory()
        {
            return Path.Combine(GetRootDirectory(), "settings", "initial_configs");
        }


        public static string GetTenantsConfigurationFile()
        {
            return Path.Combine(ActiveConfigDirectory(), "tenants.json");
        }

        public static string GetTenantsInitialConfigurationFile()
        {
            return Path.Combine(Settings_InitialConfigDirectory(), "tenants_initial.json");
        }
        public static string GetClusterInitialConfigurationFile()
        {
            return Path.Combine(Settings_InitialConfigDirectory(), "cluster_initial.json");
        }

        public static string GetStorageInitialConfigurationFile()
        {
            return Path.Combine(Settings_InitialConfigDirectory(), "storage_initial.json");
        }


        public static string GetStorageConfigurationFile()
        {
            return Path.Combine(ActiveConfigDirectory(), "storage_config.json");
        }

        public static string GetUsersConfigurationFile()
        {
            return Path.Combine(ActiveConfigDirectory(), "users_config.json");
        }

        public static string GetClustersConfigurationFile()
        {
            return Path.Combine(ActiveConfigDirectory(), "clusters_config.json");
        }
    }
}
