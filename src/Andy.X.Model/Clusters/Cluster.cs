using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Clusters
{
    public class Cluster
    {

        // in-memory properties
        public long InThroughputInMB { get; set; }
        public long OutThroughputInMB { get; set; }
        public long ActiveDataIngestions { get; set; }
        public long ActiveSubscriptions { get; set; }


        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DistributionTypes ShardDistributionType { get; set; }
        public ClusterStatus Status { get; set; }

        public List<Shard> Shards { get; set; }

        public Cluster()
        {
            ShardDistributionType = DistributionTypes.Sync;
            Shards = new List<Shard>();
            Status = ClusterStatus.Starting;


            // in-memory properties
            // for testing now
            InThroughputInMB = 0;
            OutThroughputInMB = 0;
            ActiveDataIngestions = 0;
            ActiveSubscriptions = 0;
        }
    }
}
