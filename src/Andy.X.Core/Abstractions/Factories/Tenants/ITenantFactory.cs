using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants
{
    public interface ITenantFactory
    {
        Tenant CreateTenant();
        Tenant CreateTenant(string name, string digitalSignature);
        Tenant CreateTenant(string name,
            string digitalSignature,
            bool enableEncryption,
            bool isProductAutoCreate,
            bool enableAuthorization, 
            List<TenantToken> tenantTokens, 
            TenantLogging tenantLogging, 
            bool enableGeoReplication,
            string certificatePath);

        Product CreateProduct(string productName);
        Component CreateComponent(string componentName);
        Component CreateComponent(string componentName, bool allowSchemaValidation, bool allowTopicCreation, bool enableAuthorization, List<ComponentToken> tokens);
        Topic CreateTopic(string topicName);
        Topic CreateTopic(string topicName, bool isPersistent);
    }
}
