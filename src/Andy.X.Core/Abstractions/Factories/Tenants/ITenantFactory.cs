using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants
{
    public interface ITenantFactory
    {
        Tenant CreateTenant(string name, Model.Entities.Core.Tenants.TenantSettings tenantSettings);
        Tenant CreateTenant(string name, bool enableEncryption, bool isProductAutoCreate, bool enableAuthorization);

        Product CreateProduct(string productName);
        Product CreateProduct(string name, string description);

        Component CreateComponent(string componentName);
        Component CreateComponent(string componentName, string description, Model.Entities.Core.Components.ComponentSettings componentSettings);

        Topic CreateTopic(string topicName, string description);
    }
}
