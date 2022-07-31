using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Clusters
{
    public class Shard
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DistributionTypes ReplicaDistributionType { get; set; }
        public List<Replica> Replicas { get; set; }

        public Shard()
        {
            ReplicaDistributionType = DistributionTypes.Async;
            Replicas = new List<Replica>();
        }
    }
}
