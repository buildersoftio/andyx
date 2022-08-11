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
        public Tenant CreateTenant(Guid id, string name, TenantSettings tenantSettings)
        {
            return new Tenant()
            {
                Id = id,
                Name = name,
                Settings = tenantSettings
            };
        }

        public Tenant CreateTenant(string name, string digitalSignature, bool enableEncryption, bool isProductAutoCreate, bool enableAuthorization, List<TenantToken> tenantTokens, TenantLogging tenantLogging, bool enableGeoReplication, string certificatePath)
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
                    EnableGeoReplication = enableGeoReplication,
                    CertificatePath = certificatePath
                }
            };
        }

        public Product CreateProduct(string productName)
        {
            return new Product() { Id = Guid.NewGuid(), Name = productName };
        }

        public Product CreateProduct(string name, string description, string productOwner, List<string> productTeam, string productContact)
        {
            return new Product()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                ProductOwner = productOwner,
                ProductTeam = productTeam,
                ProductContact = productContact,
            };
        }


        public Component CreateComponent(string componentName)
        {
            return new Component() { Id = Guid.NewGuid(), Name = componentName };
        }
        public Component CreateComponent(Guid id, string componentName, ComponentSettings componentSettings)
        {
            return new Component()
            {
                Id = id,
                Name = componentName,
                Settings = componentSettings
            };
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

        public Topic CreateTopic(Guid id, string topicName, TopicSettings topicSettings)
        {
            return new Topic()
            {
                Id = id,
                Name = topicName,
                TopicSettings = topicSettings
            };
        }
    }
}
