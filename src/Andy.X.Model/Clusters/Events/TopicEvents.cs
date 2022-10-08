using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class TopicCreatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }

        public Entities.Core.Topics.Topic Topic { get; set; }
        public TopicSettings TopicSettings { get; set; }
    }

    public class TopicUpdatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }

        public string Name { get; set; }
        public TopicSettings TopicSettings { get; set; }
    }

    public class TopicDeletedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }

        public string Name { get; set; }
    }

    public class CurrentEntryPositionUpdatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }

        public TopicStates TopicStates { get; set; }
    }
}
