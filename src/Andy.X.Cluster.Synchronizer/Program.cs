using Andy.X.Cluster.Synchronizer.Loggers;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Andy.X.Cluster.Synchronizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XNode node = JsonConvert.DeserializeObject<XNode>(args[0]);
            if (Directory.Exists(ConfigurationLocations.GetTempClusterRootDirectory()) != true)
                Directory.CreateDirectory(ConfigurationLocations.GetTempClusterRootDirectory());

            if (Directory.Exists(ClusterLocations.GetTempClusterDirectory(node.NodeId)) != true)
            {
                Directory.CreateDirectory(ClusterLocations.GetTempClusterDirectory(node.NodeId));
                Directory.CreateDirectory(ClusterLocations.GetTempClusterMsgsDirectory(node.NodeId));
                Directory.CreateDirectory(ClusterLocations.GetTempClusterAcksDirectory(node.NodeId));
            }

            Logger.Log($"Cluster Synchronizer for '{node.NodeId}' and '{node.Host}:{node.Post}' is online");
            // TODO: continue with the implementation when Producers will store messages into node and StorageSynchronizer will store the message.
            Console.ReadLine();
        }
    }
}
