using System.IO;

namespace Buildersoft.Andy.X.IO.Locations
{
    public static class ClusterLocations
    {
        public static string GetTempClusterDirectory(string nodeId)
        {
            return Path.Combine(ConfigurationLocations.GetTempDistributedClusterRootDirectory(), nodeId);
        }

        public static string GetTempClusterMsgsDirectory(string nodeId)
        {
            return Path.Combine(GetTempClusterDirectory(nodeId), "msgs");
        }

        public static string GetClusterChangeLogState(string nodeId)
        {
            return Path.Combine(GetTempClusterDirectory(nodeId), $"{nodeId}_changelog_state.andx");
        }

        public static string GetTempClusterAcksDirectory(string nodeId)
        {
            return Path.Combine(GetTempClusterDirectory(nodeId), "acks");
        }
    }
}
