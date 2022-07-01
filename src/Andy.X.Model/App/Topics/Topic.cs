using System;

namespace Buildersoft.Andy.X.Model.App.Topics
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public TopicSettings TopicSettings { get; set; }

        public TopicStates TopicStates { get; set; }

        public Topic()
        {
            TopicSettings = new TopicSettings();
            TopicStates = new TopicStates();
        }
    }
}
