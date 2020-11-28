using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Components
{
    public class ComponentMemoryRepository : IComponentRepository
    {
        private readonly ConcurrentDictionary<string, Component> _component;
        public ComponentMemoryRepository(ConcurrentDictionary<string, Component> component)
        {
            _component = component;
        }

        public bool Add(Component component)
        {
            return _component.TryAdd(component.Name, component);
        }

        public Component Get(string componentName)
        {
            if (_component.ContainsKey(componentName))
                return _component[componentName];
            return null;
        }

        public List<Component> GetAll()
        {
            return _component.Values.ToList();
        }

        public ConcurrentDictionary<string, Component> GetComponentsConcurrent()
        {
            return _component;
        }
    }
}
