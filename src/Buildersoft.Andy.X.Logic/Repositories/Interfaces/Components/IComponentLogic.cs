using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Components
{
    public interface IComponentLogic
    {
        Component CreateComponent(string name);
        Component GetComponent(string name);
        List<Component> GetAllComponents();
    }
}
