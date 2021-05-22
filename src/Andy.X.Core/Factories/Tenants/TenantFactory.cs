using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Model.App.Tenants;
using System;

namespace Buildersoft.Andy.X.Core.Factories.Tenants
{
    public class TenantFactory : ITenantFactory
    {
        public Tenant CreateTenant()
        {
            return new Tenant();
        }

        public Tenant CreateTenant(string name, string digitalSignature)
        {
            return new Tenant() { Id = Guid.NewGuid(), Name = name, DigitalSignature = digitalSignature };
        }
    }
}
