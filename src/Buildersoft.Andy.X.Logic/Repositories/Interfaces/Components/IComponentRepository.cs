using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Components
{
    public interface IComponentRepository
    {
        bool Add(Component component);
        Component Get(string componentName);
        List<Component> GetAll();
        ConcurrentDictionary<string, Component> GetComponentsConcurrent();
    }
}
