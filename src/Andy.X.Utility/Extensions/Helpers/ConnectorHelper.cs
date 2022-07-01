namespace Buildersoft.Andy.X.Utility.Extensions.Helpers
{
    public static class ConnectorHelper
    {
        private static readonly string COLON = ":";

        public static string GetSubcriptionId(string tenant, string product, string component, string topic, string subscription)
        {
            return $"{tenant}{COLON}{product}{COLON}{component}{COLON}{topic}{COLON}{subscription}";
        }

        public static string GetTopicConnectorKey(string tenant, string product, string component, string topic)
        {
            return $"{tenant}{COLON}{product}{COLON}{component}{COLON}{topic}";
        }

        public static string GetTopicKey(string tenant, string product, string component, string topic)
        {
            return $"{tenant}{COLON}{product}{COLON}{component}{COLON}{topic}";
        }

        public static string GetTopicKeyFromSubcriptionId(this string subcriptionId)
        {
            var splitted = subcriptionId.Split(COLON);

            return GetTopicKey(splitted[0], splitted[1], splitted[2], splitted[3]);
        }

        public static (string, string, string, string) GetDetailsFromTopicKey(this string topicKey)
        {
            var splitted = topicKey.Split(COLON);

            return (splitted[0], splitted[1], splitted[2], splitted[3]);
        }
    }
}
