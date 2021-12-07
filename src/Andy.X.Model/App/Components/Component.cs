using Buildersoft.Andy.X.Model.App.Topics;
using System;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Model.App.Components
{
    public class Component
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ConcurrentDictionary<string, Topic> Topics { get; set; }

        public bool AllowSchemaValidation { get; set; }
        public bool AllowTopicCreation { get; set; }

        public Component()
        {
            Topics = new ConcurrentDictionary<string, Topic>();
            AllowSchemaValidation = false;
            AllowTopicCreation = true;
        }
    }
}
