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

        public Tenant CreateTenant(string name, Model.Entities.Core.Tenants.TenantSettings tenantSettings)
        {
            return new Tenant()
            {
                Name = name,
                Settings = tenantSettings
            };
        }

        public Tenant CreateTenant(string name, bool enableEncryption, bool isProductAutoCreate, bool enableAuthorization)
        {
            return new Tenant()
            {
                Name = name,
                Settings = new Model.Entities.Core.Tenants.TenantSettings()
                {
                    IsEncryptionEnabled = enableEncryption,
                    IsProductAutomaticCreationAllowed = isProductAutoCreate,
                    IsAuthorizationEnabled = enableAuthorization
                }
            };
        }

        public Product CreateProduct(string productName)
        {
            return new Product()
            {
                Name = productName,
                Description = ""
            };
        }

        public Product CreateProduct(string name, string description)
        {
            return new Product()
            {
                Name = name,
                Description = description
            };
        }


        public Component CreateComponent(string componentName)
        {
            return new Component()
            {
                Name = componentName,
                Settings = new Model.Entities.Core.Components.ComponentSettings
                {
                    IsAuthorizationEnabled = false,
                    IsSchemaValidationEnabled = false,
                    IsTopicAutomaticCreationAllowed = true,
                    IsSubscriptionAutomaticCreationAllowed = true,
                    IsProducerAutomaticCreationAllowed = true
                }
            };
        }

        public Component CreateComponent(string componentName, string description, Model.Entities.Core.Components.ComponentSettings componentSettings)
        {
            return new Component()
            {
                Name = componentName,
                Description = description,
                Settings = componentSettings
            };
        }

        public Topic CreateTopic(string topicName, string description)
        {
            return new Topic()
            {
                Name = topicName,
                Description = description
            };
        }
    }
}
