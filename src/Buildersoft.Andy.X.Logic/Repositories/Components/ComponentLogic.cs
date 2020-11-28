using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Components
{
    public class ComponentLogic : IComponentLogic
    {
        private readonly IComponentRepository _componentRepository;
        public ComponentLogic(IComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
        }
        public ComponentLogic(ConcurrentDictionary<string, Component> components)
        {
            _componentRepository = new ComponentMemoryRepository(components);
        }

        public Component CreateComponent(string name)
        {
            Component component = new Component() { Name = name };
            if (_componentRepository.Add(component))
                return component;
            return null;
        }

        public List<Component> GetAllComponents()
        {
            return _componentRepository.GetAll();
        }

        public Component GetComponent(string name)
        {
            return _componentRepository.Get(name);
        }
    }
}
