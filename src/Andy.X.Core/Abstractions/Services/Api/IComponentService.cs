using Buildersoft.Andy.X.Model.App.Components;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Api
{
    public interface IComponentService
    {
        Component GetComponent(string tenantName, string productName, string componentName);
        List<Component> GetComponents(string tenantName, string productName);

        string AddComponentToken(string tenantName, string productName, string componentName, ComponentToken componentToken, bool shoudGenerateToken = true);
        bool RevokeComponentToken(string tenantName, string productName, string componentName, string token);

        string AddRetentionPolicy(string tenantName, string productName, string componentName, ComponentRetention retention);
        ComponentRetention GetRetentionPolicy(string tenantName, string productName, string componentName);

        List<ComponentToken> GetComponentTokens(string tenantName, string productName, string componentName);
    }
}
