using Buildersoft.Andy.X.IO.Locations;
using System;
using System.IO;

namespace Buildersoft.Andy.X.IO.Services
{
    public static class ClusterIOService
    {
        public static bool TryCreateClusterNodeDirectory(string nodeId)
        {
            try
            {
                if (Directory.Exists(ClusterLocations.GetTempClusterDirectory(nodeId)) == true)
                {
                    // TODO Add logging later
                    return false;
                }

                Directory.CreateDirectory(ClusterLocations.GetTempClusterDirectory(nodeId));
                Directory.CreateDirectory(ClusterLocations.GetTempClusterAcksDirectory(nodeId));
                Directory.CreateDirectory(ClusterLocations.GetTempClusterMsgsDirectory(nodeId));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
