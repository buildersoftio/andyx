using Buildersoft.Andy.X.Model.Producers;
using Buildersoft.Andy.X.Model.Subscriptions;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.App.Lineage
{
    public class StreamLineage
    {
        public List<Producer> Producers { get; set; }
        public string Topic { get; set; }
        public string TopicPhysicalPath { get; set; }
        public List<Subscription> Subscriptions { get; set; }

        public StreamLineage()
        {
            Producers = new List<Producer>();
            Subscriptions = new List<Subscription>();
        }
    }
}
