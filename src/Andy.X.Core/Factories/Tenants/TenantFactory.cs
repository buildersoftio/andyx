using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using System;
using System.Collections.Generic;

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
            return new Tenant()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Settings = new TenantSettings()
                {
                    DigitalSignature = digitalSignature,
                }
            };
        }

        public Tenant CreateTenant(string name, string digitalSignature, bool enableEncryption, bool isProductAutoCreate, bool enableAuthorization, List<TenantToken> tenantTokens, TenantLogging tenantLogging, bool enableGeoReplication)
        {
            return new Tenant()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Settings = new TenantSettings()
                {
                    DigitalSignature = digitalSignature,
                    EnableEncryption = enableEncryption,
                    AllowProductCreation = isProductAutoCreate,
                    EnableAuthorization = enableAuthorization,
                    Logging = tenantLogging,
                    Tokens = tenantTokens,
                    EnableGeoReplication = enableGeoReplication
                }
            };
        }

        public Product CreateProduct(string productName)
        {
            return new Product() { Id = Guid.NewGuid(), Name = productName };
        }

        public Component CreateComponent(string componentName)
        {
            return new Component() { Id = Guid.NewGuid(), Name = componentName };
        }
        public Component CreateComponent(string componentName, bool allowSchemaValidation, bool allowTopicCreation, bool enableAuthorization, List<ComponentToken> tokens)
        {
            return new Component()
            {
                Id = Guid.NewGuid(),
                Name = componentName,
                Settings = new ComponentSettings()
                {
                    AllowSchemaValidation = allowSchemaValidation,
                    AllowTopicCreation = allowTopicCreation,
                    EnableAuthorization = enableAuthorization,
                    Tokens = tokens
                }
            };
        }

        public Topic CreateTopic(string topicName)
        {
            return new Topic() { Id = Guid.NewGuid(), Name = topicName };
        }

        public Topic CreateTopic(string topicName, bool isPersistent)
        {
            return new Topic() { Id = Guid.NewGuid(), Name = topicName, TopicSettings = new TopicSettings() { IsPersistent = isPersistent } };
        }
    }
}
