using Buildersoft.Andy.X.Model.Clusters;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Configurations
{

    public class ClusterConfiguration
    {
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DistributionTypes DistributionType { get; set; }

        public List<Shard> Shards { get; set; }
        public ClusterConfiguration()
        {
            Shards = new List<Shard>();
            DistributionType = DistributionTypes.Sync;
        }
    }

    public class Shard
    {
        public List<Replica> Replicas { get; set; }

        public Shard()
        {
            Replicas = new List<Replica>();
        }
    }

    public class Replica
    {
        public string NodeId { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public NodeConnectionType ConnectionType { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ReplicaTypes Type { get; set; }


        public Replica()
        {
            NodeId = "standalone_01";
            Type = ReplicaTypes.MainOrWorker;
            ConnectionType = NodeConnectionType.NON_SSL;
        }
    }
}
