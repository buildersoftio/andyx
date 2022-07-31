using Buildersoft.Andy.X.Model.Clusters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Configurations
{

    public class ClusterConfiguration
    {
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DistributionTypes ShardDistributionType { get; set; }

        public List<Shard> Shards { get; set; }
        public ClusterConfiguration()
        {
            ShardDistributionType = DistributionTypes.Sync;
            Shards = new List<Shard>();
        }
    }
}
