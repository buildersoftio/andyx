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
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Configurations\\dataStorages_config.json";
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Configurations"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Configurations");

            if (!File.Exists(filePath))
                return new List<DataStorage>();

            return File.ReadAllText(filePath).JsonToObject<List<DataStorage>>();
        }

        public static bool SetDataStoragesInConfig(List<DataStorage> dataStorages)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Configurations\\dataStorages_config.json";
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Configurations"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Configurations");

            if (dataStorages != null)
                File.WriteAllText(filePath, dataStorages.ToPrettyJson());
            else
                throw new Exception("dataStorages can not be null");

            return true;
        }
    }
}
