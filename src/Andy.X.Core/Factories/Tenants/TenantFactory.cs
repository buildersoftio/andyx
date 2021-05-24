using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
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

        public Product CreateProduct(string productName)
        {
            return new Product() { Id = Guid.NewGuid(), Name = productName };
        }

        public Component CreateComponent(string componentName)
        {
            return new Component() { Id = Guid.NewGuid(), Name = componentName };
        }

        public Topic CreateTopic(string topicName)
        {
            return new Topic() { Id = Guid.NewGuid(), Name = topicName };
        }
    }
}
