namespace Buildersoft.Andy.X.Utility.Extensions.Helpers
{
    public static class ConnectorHelper
    {
        public static string GetTopicConnectorKey(string tenant, string product, string component, string topic)
        {
            return $"{tenant}:{product}:{component}:{topic}";
        }
    }
}
