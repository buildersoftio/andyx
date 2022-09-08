using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Core;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Subscriptions;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static RocksDbSharp.ColumnFamilies;

namespace Buildersoft.Andy.X.Core.Services.CoreState
{
    public class CoreService : ICoreService
    {
        private readonly ILogger<CoreService> _logger;
        private readonly ICoreRepository _coreRepository;
        private readonly StorageConfiguration _storageConfiguration;

        public CoreService(ILogger<CoreService> logger, ICoreRepository coreRepository, StorageConfiguration storageConfiguration)
        {
            _logger = logger;
            _coreRepository = coreRepository;
            _storageConfiguration = storageConfiguration;

            if (_coreRepository.GetCoreStateContext().Database.EnsureCreated())
            {
                // adding tenants from config files.
                _logger.LogInformation("Starting cluster for the first time");
                _logger.LogInformation("Initial configuration in process ...");
                var tenantsFromSettings = JsonConvert.DeserializeObject<List<TenantConfiguration>>(File.ReadAllText(ConfigurationLocations.GetTenantsInitialConfigurationFile()));
                foreach (var tenant in tenantsFromSettings)
                {
                    CreateTenant(tenant.Name, tenant.Settings.IsProductAutomaticCreationAllowed, tenant.Settings.IsEncryptionEnabled, tenant.Settings.IsAuthorizationEnabled);
                    foreach (var product in tenant.Products)
                    {
                        CreateProduct(tenant.Name, product.Name, "initial");
                        foreach (var component in product.Components)
                        {
                            CreateComponent(tenant.Name, product.Name, component.Name, "initial", component.Settings.IsTopicAutomaticCreationAllowed, component.Settings.IsSchemaValidationEnabled, component.Settings.IsAuthorizationEnabled, component.Settings.IsSubscriptionAutomaticCreationAllowed);
                            foreach (var topic in component.Topics)
                            {
                                CreateTopic(tenant.Name, product.Name, component.Name, topic.Name, "initial");
                            }
                        }
                    }
                }
                _logger.LogInformation("Initial configuration are done");
            }
        }

        public bool CreateTenant(string tenantName)
        {
            return CreateTenant(tenantName, true, false, false);
        }
        public bool CreateTenant(string tenantName, bool isProductCreation, bool isEncryptionEnabled, bool isAuthorizationEnabled)
        {
            if (_coreRepository.GetTenant(tenantName) is not null)
                return false;  // already exists

            var tenantToRegister = new Tenant()
            {
                Name = tenantName,
                IsActive = true,
                CreatedBy = "system",
                CreatedDate = DateTimeOffset.Now
            };

            _coreRepository.AddTenant(tenantToRegister);
            _coreRepository.SaveChanges();

            // add default settings
            var defaultTenantSettings = new TenantSettings()
            {
                TenantId = tenantToRegister.Id,
                IsAuthorizationEnabled = isAuthorizationEnabled,
                IsProductAutomaticCreationAllowed = isProductCreation,
                IsEncryptionEnabled = isEncryptionEnabled,

                CreatedBy = "system",
                CreatedDate = DateTimeOffset.Now
            };

            _coreRepository.AddTenantSettings(defaultTenantSettings);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteTenant(string tenantName)
        {
            var tenant = _coreRepository.GetTenant(tenantName);
            if (tenant is null)
                return false;  // tenant doesnot exists

            _coreRepository.SoftDeleteTenant(tenant.Id);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool ActivateTenant(string tenantName)
        {
            var tenant = _coreRepository.GetTenant(tenantName);
            if (tenant is null)
                return false;  // tenant doesnot exists

            if (tenant.IsActive == true)
                return true;


            tenant.IsActive = true;
            tenant.UpdatedDate = DateTimeOffset.Now;
            tenant.UpdatedBy = "SYSTEM";

            _coreRepository.EditTenant(tenant);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeactivateTenant(string tenantName)
        {
            var tenant = _coreRepository.GetTenant(tenantName);
            if (tenant is null)
                return false;  // tenant doesnot exists

            if (tenant.IsActive == false)
                return true;


            tenant.IsActive = false;
            tenant.UpdatedDate = DateTimeOffset.Now;
            tenant.UpdatedBy = "SYSTEM";

            _coreRepository.EditTenant(tenant);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool UpdateTenantSettings(string tenantName, bool isProductCreation, bool isEncryptionEnabled, bool isAuthorizationEnabled)
        {
            var tenant = _coreRepository.GetTenant(tenantName);
            if (tenant is null)
                return false;  // tenant doesnot exists

            var currentTenantSettings = _coreRepository.GetTenantSettings(tenant.Id);
            if (currentTenantSettings is null)
                return false;   // if is here, something is really wrong...

            currentTenantSettings.IsEncryptionEnabled = isEncryptionEnabled;
            currentTenantSettings.IsAuthorizationEnabled = isAuthorizationEnabled;
            currentTenantSettings.IsProductAutomaticCreationAllowed = isProductCreation;
            currentTenantSettings.UpdatedDate = DateTimeOffset.Now;
            currentTenantSettings.UpdatedBy = "SYSTEM";

            _coreRepository.EditTenantSettings(currentTenantSettings);
            _coreRepository.SaveChanges();

            return true;
        }


        public bool CreateTenantRetention(string tenant, string name, RetentionType retentionType, long timeToLive)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            // checking if there is a the same retention type.
            var retention = _coreRepository
                .GetTenantRetentions(currentTenant.Id)
                .Where(x => x.Type == retentionType)
                .FirstOrDefault();

            if (retention is not null)
                return false;

            var tenantRetentionToRegister = new TenantRetention()
            {
                TenantId = currentTenant.Id,
                Name = name,
                Type = retentionType,
                TimeToLiveInMinutes = timeToLive,

                CreatedBy = "system",
                CreatedDate = DateTimeOffset.Now
            };

            _coreRepository.AddTenantRetention(tenantRetentionToRegister);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool UpdateTenantRetention(string tenant, long retentionId, string name, long timeToLive)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentTenantRetention = _coreRepository.GetTenantRetention(retentionId);
            if (currentTenantRetention is null)
                return false;   // this retention does not exists.

            if (currentTenant.Id != currentTenantRetention.TenantId)
                return false; // wrong request, this retention is not part of this tenant

            currentTenantRetention.Name = name;
            currentTenantRetention.TimeToLiveInMinutes = timeToLive;
            currentTenantRetention.UpdatedDate = DateTimeOffset.Now;
            currentTenantRetention.UpdatedBy = "SYSTEM";

            _coreRepository.EditTenantRetention(currentTenantRetention);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteTenantRetention(string tenant, long retentionId)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentTenantRetention = _coreRepository.GetTenantRetention(retentionId);
            if (currentTenantRetention is null)
                return false;   // this retention does not exists.

            if (currentTenant.Id != currentTenantRetention.TenantId)
                return false; // wrong request, this retention is not part of this tenant

            _coreRepository.DeleteTenantRetention(currentTenantRetention.Id);
            _coreRepository.SaveChanges();

            return true;
        }


        public bool CreateTenantToken(string tenant, string description, DateTimeOffset expireDate, List<TenantTokenRole> tenantTokenRoles, out Guid id, out string secret)
        {
            id = Guid.Empty;
            secret = "";

            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            secret = "GENERATED_SECRET";
            string encryptedSecret = "ENCRYPTED_SECRET";

            var tenantTokenToRegister = new TenantToken()
            {
                TenantId = currentTenant.Id,
                Description = description,
                IssuedDate = DateTimeOffset.Now,
                ExpireDate = expireDate,
                Secret = encryptedSecret,
                IsActive = true,
                _Roles = tenantTokenRoles.ToJson(),

                CreatedBy = "system",
                CreatedDate = DateTimeOffset.Now
            };

            _coreRepository.AddTenantToken(tenantTokenToRegister);
            _coreRepository.SaveChanges();

            id = tenantTokenToRegister.Id;

            return true;
        }
        public bool RevokeTenantToken(string tenant, Guid id)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentTenantTenant = _coreRepository.GetTenantToken(id);
            if (currentTenantTenant is null)
                return false;   // this retention does not exists.

            if (currentTenant.Id != currentTenantTenant.TenantId)
                return false; // wrong request, this retention is not part of this tenant

            currentTenantTenant.IsActive = false;
            currentTenantTenant.UpdatedBy = "SYSTEM";
            currentTenantTenant.UpdatedDate = DateTimeOffset.Now;

            _coreRepository.EditTenantToken(currentTenantTenant);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteTenantToken(string tenant, Guid id)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentTenantTenant = _coreRepository.GetTenantToken(id);
            if (currentTenantTenant is null)
                return false;   // this retention does not exists.

            if (currentTenant.Id != currentTenantTenant.TenantId)
                return false; // wrong request, this retention is not part of this tenant

            _coreRepository.DeleteTenantToken(id);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool CreateProduct(string tenant, string productName, string description)
        {
            return CreateProduct(tenant, productName, description, false);
        }
        public bool CreateProduct(string tenant, string productName, string description, bool isAuthorizationEnabled)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, productName);
            if (currentProduct is not null)
                return false; // product exists

            var productToCreate = new Product()
            {
                TenantId = currentTenant.Id,
                Name = productName,
                Description = description,

                CreatedBy = "system",
                CreatedDate = DateTimeOffset.Now
            };

            _coreRepository.AddProduct(productToCreate);
            _coreRepository.SaveChanges();


            // add default settings
            var defaultSettings = new ProductSettings()
            {
                ProductId = productToCreate.Id,
                IsAuthorizationEnabled = isAuthorizationEnabled,

                CreatedBy = "system",
                CreatedDate = DateTimeOffset.Now
            };

            _coreRepository.AddProductSettings(defaultSettings);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteProduct(string tenant, string product)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product exists

            _coreRepository.SoftDeleteProduct(currentProduct.Id);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool UpdateProductSettings(string tenant, string product, bool isAuthorizationEnabled)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product exists

            var currentSettings = _coreRepository.GetProductSettings(currentProduct.Id);

            currentSettings.IsAuthorizationEnabled = isAuthorizationEnabled;
            currentSettings.UpdatedDate = DateTimeOffset.Now;
            currentSettings.UpdatedBy = "SYSTEM";

            _coreRepository.EditProductSettings(currentSettings);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool UpdateProduct(string tenant, string product, string description)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            currentProduct.Description = description;
            currentProduct.UpdatedDate = DateTimeOffset.Now;
            currentProduct.UpdatedBy = "SYSTEM";

            _coreRepository.EditProduct(currentProduct);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool CreateProductToken(string tenant, string product, string description, DateTimeOffset expireDate, List<ProductTokenRole> productTokenRoles, out Guid id, out string secret)
        {
            id = Guid.Empty;
            secret = "";

            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            secret = "GENERATE_SECRET";
            string encryptedSecret = "GENERATE_SECRET";

            var tokenToRegister = new ProductToken()
            {

                Description = description,
                ExpireDate = expireDate,
                IsActive = true,
                IssuedDate = DateTimeOffset.Now,
                Secret = encryptedSecret,
                ProductId = currentProduct.Id,

                _Roles = productTokenRoles.ToJson(),

                CreatedBy = "system",
                CreatedDate = DateTimeOffset.Now
            };

            _coreRepository.AddProductToken(tokenToRegister);
            _coreRepository.SaveChanges();

            id = tokenToRegister.Id;

            return true;
        }
        public bool RevokeProductToken(string tenant, string product, Guid id)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentProductToken = _coreRepository.GetProductToken(id);

            if (currentProduct.Id != currentProductToken.ProductId)
                return false; // token is not part of the product

            currentProductToken.IsActive = false;
            currentProductToken.UpdatedDate = DateTimeOffset.Now;
            currentProductToken.UpdatedBy = "SYSTEM";

            _coreRepository.EditProductToken(currentProductToken);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteProductToken(string tenant, string product, Guid id)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentProductToken = _coreRepository.GetProductToken(id);

            if (currentProduct.Id != currentProductToken.ProductId)
                return false; // token is not part of the product

            _coreRepository.DeleteProductToken(id);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool CreateProductRetention(string tenant, string product, string name, RetentionType retentionType, long timeToLive)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            // checking if there is a the same retention type.
            var retention = _coreRepository
                .GetProductRetentions(currentProduct.Id)
                .Where(x => x.Type == retentionType)
                .FirstOrDefault();

            if (retention is not null)
                return false;

            var productRetentionToRegister = new ProductRetention()
            {
                ProductId = currentProduct.Id,
                Type = retentionType,
                Name = name,
                TimeToLiveInMinutes = timeToLive,

                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "SYSTEM"
            };

            _coreRepository.AddProductRetention(productRetentionToRegister);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool UpdateProductRetention(string tenant, string product, long retentionId, string name, long timeToLive)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentProductRetention = _coreRepository.GetProductRetention(retentionId);
            if (currentProductRetention is null)
                return false;   // retention doesnot exists

            if (currentProductRetention.ProductId != currentProduct.Id)
                return false;   // this retention is not part of this product

            currentProductRetention.Name = name;
            currentProductRetention.TimeToLiveInMinutes = timeToLive;
            currentProductRetention.UpdatedDate = DateTimeOffset.UtcNow;
            currentProductRetention.UpdatedBy = "SYSTEM";

            _coreRepository.EditProductRetention(currentProductRetention);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteProductRetention(string tenant, string product, long retentionId)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentProductRetention = _coreRepository.GetProductRetention(retentionId);
            if (currentProductRetention is null)
                return false;   // retention doesnot exists

            if (currentProductRetention.ProductId != currentProduct.Id)
                return false;   // this retention is not part of this product

            _coreRepository.DeleteProductRetention(retentionId);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool CreateComponent(string tenant, string product, string componentName, string description)
        {
            return CreateComponent(tenant, product, componentName, description, true, false, false, true);
        }
        public bool CreateComponent(string tenant, string product, string componentName, string description, bool isTopicAutomaticCreation, bool isSchemaValidationEnabled, bool isAuthorizationEnabled, bool isSubscriptionAllowToCreate)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, componentName);
            if (currentComponent is not null)
                return false; // component already exists

            var componentToRegister = new Component()
            {
                Name = componentName,
                Description = description,
                ProductId = currentProduct.Id,

                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "SYSTEM"
            };

            _coreRepository.AddComponent(componentToRegister);
            _coreRepository.SaveChanges();


            // add default settings
            var defaultSettings = new ComponentSettings()
            {
                ComponentId = componentToRegister.Id,
                IsTopicAutomaticCreationAllowed = isTopicAutomaticCreation,
                IsSchemaValidationEnabled = isSchemaValidationEnabled,
                IsAuthorizationEnabled = isAuthorizationEnabled,
                IsSubscriptionAutomaticCreationAllowed = isSubscriptionAllowToCreate,

                CreatedBy = "system",
                CreatedDate = DateTimeOffset.Now
            };

            _coreRepository.AddComponentSettings(defaultSettings);
            _coreRepository.SaveChanges();


            return true;
        }
        public bool UpdateComponent(string tenant, string product, string component, string description)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            currentComponent.Description = description;
            currentComponent.UpdatedDate = DateTimeOffset.UtcNow;
            currentComponent.UpdatedBy = "SYSTEM";

            return true;
        }
        public bool DeleteComponent(string tenant, string product, string component)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentSettings = _coreRepository.GetComponentSettings(currentComponent.Id);
            if (currentSettings is null)
                return false; // settings doesnot exists, something is really wrong.!

            currentSettings.IsMarkedForDeletion = true;
            currentSettings.UpdatedDate = DateTimeOffset.UtcNow;
            currentSettings.UpdatedBy = "SYSTEM";

            _coreRepository.EditComponentSettings(currentSettings);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool UpdateComponentSettings(string tenant, string product, string componentName, bool isTopicAutomaticCreation, bool isSchemaValidationEnabled, bool isAuthorizationEnabled, bool isSubscriptionAllowToCreate)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, componentName);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentSettings = _coreRepository.GetComponentSettings(currentComponent.Id);
            if (currentSettings is null)
                return false; // settings doesnot exists, something is really wrong.!

            currentSettings.IsSchemaValidationEnabled = isSchemaValidationEnabled;
            currentSettings.IsTopicAutomaticCreationAllowed = isTopicAutomaticCreation;
            currentSettings.IsAuthorizationEnabled = isAuthorizationEnabled;
            currentSettings.IsSubscriptionAutomaticCreationAllowed = isSubscriptionAllowToCreate;

            currentSettings.UpdatedDate = DateTimeOffset.UtcNow;
            currentSettings.UpdatedBy = "SYSTEM";

            _coreRepository.EditComponentSettings(currentSettings);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool CreateComponentToken(string tenant, string product, string component, string description, string issuedFor, DateTimeOffset expireDate, List<ComponentTokenRole> componentTokenRoles, out Guid id, out string secret)
        {
            id = Guid.Empty;
            secret = "";

            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            secret = "GENERATE_SECRET";
            var encryptedSecret = "ENCRYPTED_SECRET";

            var newToken = new ComponentToken()
            {
                ComponentId = currentComponent.Id,
                Description = description,
                IsActive = true,
                Secret = encryptedSecret,
                IssuedDate = DateTimeOffset.Now,
                ExpireDate = expireDate,
                IssuedFor = issuedFor,

                _Roles = componentTokenRoles.ToJson(),

                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "SYSTEM"
            };

            _coreRepository.AddComponentToken(newToken);
            _coreRepository.SaveChanges();

            id = newToken.Id;

            return true;
        }
        public bool RevokeComponentToken(string tenant, string product, string component, Guid id)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentToken = _coreRepository.GetComponentToken(id);
            if (currentToken is null)
                return false; // token doesnot exits

            if (currentToken.ComponentId != currentComponent.Id)
                return false; // this token is not of this component

            currentToken.IsActive = false;
            currentToken.UpdatedDate = DateTimeOffset.UtcNow;
            currentToken.UpdatedBy = "SYSTEM";

            _coreRepository.EditComponentToken(currentToken);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteComponentToken(string tenant, string product, string component, Guid id)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentToken = _coreRepository.GetComponentToken(id);
            if (currentToken is null)
                return false; // token doesnot exits

            if (currentToken.ComponentId != currentComponent.Id)
                return false; // this token is not of this component

            _coreRepository.DeleteComponentToken(id);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool CreateComponentRetention(string tenant, string product, string component, string name, RetentionType retentionType, long timeToLive)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            // checking if there is a the same retention type.
            var retention = _coreRepository
                .GetComponentRetentions(currentComponent.Id)
                .Where(x => x.Type == retentionType)
                .FirstOrDefault();

            if (retention is not null)
                return false;

            var newRetention = new ComponentRetention()
            {
                ComponentId = currentComponent.Id,
                Name = name,
                TimeToLiveInMinutes = timeToLive,
                Type = retentionType,

                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "SYSTEM"
            };

            _coreRepository.AddComponentRetention(newRetention);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool UpdateComponentRetention(string tenant, string product, string component, long retentionId, string name, long timeToLive)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentComponentRetention = _coreRepository.GetComponentRetention(retentionId);
            if (currentComponentRetention is null)
                return false; // retention doesnot exists for this component

            if (currentComponentRetention.ComponentId != currentComponent.Id)
                return false; // this retention is not part of this component

            currentComponentRetention.Name = name;
            currentComponentRetention.TimeToLiveInMinutes = timeToLive;
            currentComponentRetention.UpdatedDate = DateTimeOffset.UtcNow;
            currentComponentRetention.UpdatedBy = "SYSTEM";

            _coreRepository.EditComponentRetention(currentComponentRetention);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteComponentRetention(string tenant, string product, string component, long retentionId)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentComponentRetention = _coreRepository.GetComponentRetention(retentionId);
            if (currentComponentRetention is null)
                return false; // retention doesnot exists for this component

            if (currentComponentRetention.ComponentId != currentComponent.Id)
                return false; // this retention is not part of this component

            _coreRepository.DeleteComponentRetention(retentionId);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool CreateTopic(string tenant, string product, string component, string topic, string description)
        {
            var topicSettings = new TopicSettings()
            {
                WriteBufferSizeInBytes = _storageConfiguration.DefaultWriteBufferSizeInBytes,
                MaxWriteBufferNumber = _storageConfiguration.DefaultMaxWriteBufferNumber,
                MaxWriteBufferSizeToMaintain = _storageConfiguration.DefaultMaxWriteBufferSizeToMaintain,
                MinWriteBufferNumberToMerge = _storageConfiguration.DefaultMinWriteBufferNumberToMerge,
                MaxBackgroundCompactionsThreads = _storageConfiguration.DefaultMaxBackgroundCompactionsThreads,
                MaxBackgroundFlushesThreads = _storageConfiguration.DefaultMaxBackgroundFlushesThreads,

                CreatedBy = "SYSTEM",
                CreatedDate = DateTimeOffset.UtcNow,
            };

            return CreateTopic(tenant, product, component, topic, description, topicSettings);
        }
        public bool UpdateTopic(string tenant, string product, string component, string topic, string description)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentTopic = _coreRepository.GetTopic(currentComponent.Id, topic);
            if (currentTopic is null)
                return false; // topic doesnot exists

            if (currentTopic.ComponentId != currentComponent.Id)
                return false; // this topic is not part of the component

            currentTopic.Description = description;
            currentTopic.UpdatedDate = DateTimeOffset.UtcNow;
            currentTopic.UpdatedBy = "SYSTEM";

            _coreRepository.EditTopic(currentTopic);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteTopic(string tenant, string product, string component, string topic)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentTopic = _coreRepository.GetTopic(currentComponent.Id, topic);
            if (currentTopic is null)
                return false; // topic doesnot exists

            if (currentTopic.ComponentId != currentComponent.Id)
                return false; // this topic is not part of the component

            currentTopic.IsMarkedForDeletion = true;
            currentTopic.UpdatedDate = DateTimeOffset.UtcNow;
            currentTopic.UpdatedBy = "SYSTEM";

            _coreRepository.EditTopic(currentTopic);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool CreateSubscription(string tenant, string product, string component, string topic, string subscription, Model.Subscriptions.SubscriptionType type, Model.Subscriptions.SubscriptionMode mode, Model.Subscriptions.InitialPosition initialPosition)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentTopic = _coreRepository.GetTopic(currentComponent.Id, topic);
            if (currentTopic is null)
                return false; // topic doesnot exists

            var currentSubscription = _coreRepository.GetSubscription(currentTopic.Id, subscription);
            if (currentSubscription is not null)
                return false; // this subscription already exists in this topic

            var newSubscription = new Subscription()
            {
                TopicId = currentTopic.Id,
                Name = subscription,
                InitialPosition = initialPosition,
                SubscriptionMode = mode,
                SubscriptionType = type,

                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "SYSTEM"
            };

            _coreRepository.AddSubscription(newSubscription);
            _coreRepository.SaveChanges();

            return true;
        }
        public bool DeleteSubscription(string tenant, string product, string component, string topic, string subscription)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentTopic = _coreRepository.GetTopic(currentComponent.Id, topic);
            if (currentTopic is null)
                return false; // topic doesnot exists

            var currentSubscription = _coreRepository.GetSubscription(currentTopic.Id, subscription);
            if (currentSubscription is null)
                return false; // this subscription does not exits

            currentSubscription.IsMarkedForDeletion = true;
            currentSubscription.UpdatedDate = DateTimeOffset.UtcNow;
            currentSubscription.UpdatedBy = "SYSTEM";

            _coreRepository.EditSubscription(currentSubscription);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool CreateTopic(string tenant, string product, string component, string topic, string description, TopicSettings topicSettings)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentTopic = _coreRepository.GetTopic(currentComponent.Id, topic);
            if (currentTopic is not null)
                return false; // topic already exists

            var newTopic = new Topic()
            {
                Name = topic,
                Description = description,
                ComponentId = currentComponent.Id,

                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "SYSTEM"
            };

            _coreRepository.AddTopic(newTopic);
            _coreRepository.SaveChanges();

            // Adding Topic Settings
            topicSettings.TopicId = newTopic.Id;
            _coreRepository.AddTopicSettings(topicSettings);
            _coreRepository.SaveChanges();

            return true;
        }

        public bool UpdateTopicSettings(string tenant, string product, string component, string topic, TopicSettings topicSettings)
        {
            var currentTenant = _coreRepository.GetTenant(tenant);
            if (currentTenant is null)
                return false;  // tenant doesnot exists

            var currentProduct = _coreRepository.GetProduct(currentTenant.Id, product);
            if (currentProduct is null)
                return false; // product doesnot exists

            var currentComponent = _coreRepository.GetComponent(currentTenant.Id, currentProduct.Id, component);
            if (currentComponent is null)
                return false; // component doesnot exists

            var currentTopic = _coreRepository.GetTopic(currentComponent.Id, topic);
            if (currentTopic is null)
                return false; // topic doesnot exists

            if (currentTopic.ComponentId != currentComponent.Id)
                return false; // this topic is not part of the component

            var currentTopicSettings = _coreRepository.GetTopicSettings(currentTopic.Id);

            currentTopicSettings.WriteBufferSizeInBytes = topicSettings.WriteBufferSizeInBytes;
            currentTopicSettings.MaxWriteBufferNumber = topicSettings.MaxWriteBufferNumber;
            currentTopicSettings.MaxWriteBufferSizeToMaintain = topicSettings.MaxWriteBufferSizeToMaintain;
            currentTopicSettings.MinWriteBufferNumberToMerge = topicSettings.MinWriteBufferNumberToMerge;
            currentTopicSettings.MaxBackgroundCompactionsThreads = topicSettings.MaxBackgroundCompactionsThreads;
            currentTopicSettings.MaxBackgroundFlushesThreads = topicSettings.MaxBackgroundFlushesThreads;

            currentTopicSettings.UpdatedDate = DateTimeOffset.UtcNow;
            currentTopicSettings.UpdatedBy = "SYSTEM";

            _coreRepository.EditTopic(currentTopic);
            _coreRepository.SaveChanges();

            return true;
        }
    }
}
