using Buildersoft.Andy.X.Model.App.Tenants;
namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants
{
    public interface ITenantFactory
    {
        Tenant CreateTenant();
        Tenant CreateTenant(string name, string digitalSignature);
    }
}
