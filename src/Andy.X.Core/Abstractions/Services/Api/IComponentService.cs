using Buildersoft.Andy.X.Model.App.Components;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Api
{
    public interface IComponentService
    {
        Component GetComponent(string tenantName, string productName, string componentName);
        List<Component> GetComponents(string tenantName, string productName);

        string AddComponentToken(string tenantName, string productName, string componentName, ComponentToken componentToken);

        List<ComponentToken> GetComponentTokens(string tenantName, string productName, string componentName);
    }
}
