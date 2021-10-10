using Buildersoft.Andy.X.Model.App.Topics;
using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Topics
{
    public class TopicUpdatedDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public TopicSettings TopicSettings { get; set; }

        public Schema Schema { get; set; }
    }
}
