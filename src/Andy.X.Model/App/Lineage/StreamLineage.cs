using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Producers;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.App.Lineage
{
    public class StreamLineage
    {
        public List<Producer> Producers { get; set; }
        public string Topic { get; set; }
        public string TopicPhysicalPath { get; set; }
        public List<Consumer> Consumers { get; set; }

        public StreamLineage()
        {
            Producers = new List<Producer>();
            Consumers = new List<Consumer>();
        }
    }
}
