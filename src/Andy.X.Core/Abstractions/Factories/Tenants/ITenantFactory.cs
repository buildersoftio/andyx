using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants
{
    public interface ITenantFactory
    {
        Tenant CreateTenant();
        Tenant CreateTenant(string name, string digitalSignature);
        Tenant CreateTenant(string name, string digitalSignature, bool enableEncryption, bool IsProductAutoCreate);

        Product CreateProduct(string productName);
        Component CreateComponent(string componentName);
        Component CreateComponent(string componentName, bool allowSchemaValidation, bool allowTopicCreation);
        Topic CreateTopic(string topicName);
        Topic CreateTopic(string topicName, bool isPersistent);
    }
}
