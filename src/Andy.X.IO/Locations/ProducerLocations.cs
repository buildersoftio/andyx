using System.IO;

namespace Buildersoft.Andy.X.IO.Locations
{
    public static class ProducerLocations
    {
        public static string GetProducerDirectory(string tenant, string product, string component, string topic, string producer)
        {
            return Path.Combine(TenantLocations.GetProducerRootDirectory(tenant, product, component, topic), producer);
        }
        public static string GetProducerLoggingDirectory(string tenant, string product, string component, string topic, string producer)
        {
            return Path.Combine(GetProducerDirectory(tenant, product, component, topic, producer), "logs");
        }
    }
}
