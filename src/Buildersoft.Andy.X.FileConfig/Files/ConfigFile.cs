using Buildersoft.Andy.X.Data.Model.DataStorages;
using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using Buildersoft.Andy.X.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Buildersoft.Andy.X.IO.Files
{
    public static class ConfigFile
    {
        public static List<DataStorage> GetDataStoragesFromConfig()
        {
            string configRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations");
            string dataStorageFileConfigLocation = Path.Combine(configRoot, "dataStorages_config.json");
            if (!Directory.Exists(configRoot))
                Directory.CreateDirectory(configRoot);

            if (!File.Exists(dataStorageFileConfigLocation))
                return new List<DataStorage>();

            return File.ReadAllText(dataStorageFileConfigLocation).JsonToObject<List<DataStorage>>();
        }

        public static bool SetDataStoragesInConfig(List<DataStorage> dataStorages)
        {
            string configRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations");
            string dataStorageFileConfigLocation = Path.Combine(configRoot, "dataStorages_config.json");

            if (!Directory.Exists(configRoot))
                Directory.CreateDirectory(configRoot);

            if (dataStorages != null)
                File.WriteAllText(dataStorageFileConfigLocation, dataStorages.ToPrettyJson());
            else
                throw new Exception("dataStorages can not be null");

            return true;
        }
    }
}
