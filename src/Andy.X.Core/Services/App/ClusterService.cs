using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace Buildersoft.Andy.X.Core.Services.App
{
    public class ClusterService
    {
        private readonly ILogger<ClusterService> _logger;
        private readonly ClusterConfiguration _clusterConfiguration;
        private readonly ConcurrentDictionary<string, Process> clusterSyncronizers;

        public ClusterService(ILogger<ClusterService> logger, ClusterConfiguration clusterConfiguration)
        {
            _logger = logger;
            _clusterConfiguration = clusterConfiguration;
            clusterSyncronizers = new ConcurrentDictionary<string, Process>();

            // InitializeCluster();
        }

        public void InitializeCluster()
        {
            foreach (var node in _clusterConfiguration.Nodes)
            {
                if (Environment.GetEnvironmentVariable("ANDYX_NODE_ID") != node.NodeId)
                {
                    _logger.LogInformation($"Adding node '{node.Host}' to the cluster '{_clusterConfiguration.Name}'");
                    // run the cluster syncroization process

                    var clusterSync = new Process();
                    clusterSync.StartInfo = new ProcessStartInfo()
                    {
                        FileName = "dotnet",
                        UseShellExecute = true,
                        CreateNoWindow = true,
                    };
                    clusterSync.StartInfo.ArgumentList.Add(Path.Join(ConfigurationLocations.GetRootDirectory(), "Andy.X.Cluster.Synchronizer.dll"));
                    clusterSync.StartInfo.ArgumentList.Add(JsonConvert.SerializeObject(node));

                    clusterSync.OutputDataReceived += ClusterSync_OutputDataReceived;
                    clusterSync.Start();

                    // adding the nodes into a list, for latter if we will need them to do something...
                    clusterSyncronizers.TryAdd(node.NodeId, clusterSync);
                }
            }
        }

        private void ClusterSync_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.LogInformation(e.Data);
        }
    }
}
