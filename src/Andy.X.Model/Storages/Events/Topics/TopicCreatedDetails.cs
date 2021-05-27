using Buildersoft.Andy.X.Model.App.Topics;
using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Topics
{
    public class TopicCreatedDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Schema Schema { get; set; }
    }
}
