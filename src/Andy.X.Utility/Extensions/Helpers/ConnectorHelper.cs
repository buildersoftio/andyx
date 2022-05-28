namespace Buildersoft.Andy.X.Utility.Extensions.Helpers
{
    public static class ConnectorHelper
    {

        public static string GetSubcriptionId(string tenant, string product, string component, string topic, string subscription)
        {
            return $"{tenant}{product}{component}{topic}:{subscription}";
        }

        public static string GetTopicConnectorKey(string tenant, string product, string component, string topic)
        {
            return $"{tenant}:{product}:{component}:{topic}";
        }

        public static string GetTopicSynchronizerKey(string tenant, string product, string component, string topic)
        {
            return $"{tenant}:{product}:{component}:{topic}";
        }
    }
}
