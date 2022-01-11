using Buildersoft.Andy.X.Model.App.Topics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.App.Components
{
    public class Component
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ConcurrentDictionary<string, Topic> Topics { get; set; }

        public bool AllowSchemaValidation { get; set; }
        public bool AllowTopicCreation { get; set; }

        // TBD: tokens will be part of components;

        public List<ComponentToken> Tokens { get; set; }


        public Component()
        {
            Topics = new ConcurrentDictionary<string, Topic>();
            Tokens = new List<ComponentToken>();

            AllowSchemaValidation = false;
            AllowTopicCreation = true;
        }
    }
}
