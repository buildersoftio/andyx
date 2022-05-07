using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Buildersoft.Andy.X.IO.Readers
{
    public static class TenantIOReader
    {
        public static List<TenantConfiguration> ReadTenantsFromConfigFile()
        {

            while (true)
            {
                try
                {
                    var tenantConfigurations = File.ReadAllText(ConfigurationLocations.GetTenantsConfigurationFile()).JsonToObject<List<TenantConfiguration>>();
                    return tenantConfigurations;
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
