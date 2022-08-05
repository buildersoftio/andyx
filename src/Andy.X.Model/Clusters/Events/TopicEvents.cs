using Buildersoft.Andy.X.Model.App.Topics;
using System;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class TopicCreatedArgs
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public TopicSettings TopicSettings { get; set; }

        public TopicStates TopicStates { get; set; }

    }

    public class TopicUpdatedArgs
    {
        public string Name { get; set; }

        public TopicSettings TopicSettings { get; set; }
    }

    public class TopicDeletedArgs
    {
        public string Name { get; set; }
    }

    public class CurrentEntryPositionUpdatedArgs
    {
        public string Name { get; set; }
        public TopicStates TopicStates { get; set; }
    }
}
