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
        Tenant CreateTenant();
        Tenant CreateTenant(Guid id, string name, TenantSettings tenantSettings);
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
        Product CreateProduct(string name, string description, string productOwner, List<string> productTeam, string productContact);
        Component CreateComponent(string componentName);
        Component CreateComponent(Guid id, string componentName, ComponentSettings componentSettings);
        Component CreateComponent(string componentName, bool allowSchemaValidation, bool allowTopicCreation, bool enableAuthorization, List<ComponentToken> tokens);
        Topic CreateTopic(string topicName);
        Topic CreateTopic(Guid id, string topicName, TopicSettings topicSettings);
        Topic CreateTopic(string topicName, bool isPersistent);
    }
}
