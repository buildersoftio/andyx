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
    }
}
