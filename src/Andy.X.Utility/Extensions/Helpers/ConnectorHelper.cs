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

        public static string GetTopicKey(string tenant, string product, string component, string topic)
        {
            return $"{tenant}:{product}:{component}:{topic}";
        }

        public static (string, string, string, string) GetDetailsFromTopicKey(this string topicKey)
        {
            var splitted = topicKey.Split(":");

            return (splitted[0], splitted[1], splitted[2], splitted[3]);
        }
    }
}
