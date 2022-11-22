using System;

namespace Buildersoft.Andy.X.Model.App.Topics
{
    public class Topic
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public TopicStates TopicStates { get; set; }

        public Topic()
        {
            TopicStates = new TopicStates();
        }
    }
}
