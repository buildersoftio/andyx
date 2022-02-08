using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using System.Collections.Generic;
using System.IO;

namespace Buildersoft.Andy.X.IO.Readers
{
    public static class TenantIOReader
    {
        public static List<TenantConfiguration> ReadTenantsFromConfigFile()
        {
            return File.ReadAllText(ConfigurationLocations.GetTenantsConfigurationFile()).JsonToObject<List<TenantConfiguration>>();
        }
    }
}
