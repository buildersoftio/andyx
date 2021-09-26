using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Configurations
{
    public class ClustersConfiguration
    {
        public List<Cluster> Clusters { get; set; }
    }

    public class Cluster
    {
        public string Name { get; set; }
        public bool AllowReplication { get; set; }
        public List<XNode> XNodes { get; set; }
    }

    public class XNode
    {
        public string HostName { get; set; }

        //   List of tenants that are part of this cluster
        //   For we will not configure Tenants that are part of a cluster, all nodes connected in a cluster are part of that cluster.
        // public string[] Tenants { get; set; }

        public bool IsFailover { get; set; }

        public XNode()
        {
            IsFailover = false;
        }
    }
}
