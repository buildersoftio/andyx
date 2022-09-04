using Buildersoft.Andy.X.Model.App.Topics;
using System;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Model.App.Components
{
    public class Component
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ConcurrentDictionary<string, Topic> Topics { get; set; }

        public Entities.Core.Components.ComponentSettings Settings { get; set; }


        public Component()
        {
            Topics = new ConcurrentDictionary<string, Topic>();
            Settings = new Entities.Core.Components.ComponentSettings();
        }
    }
}
