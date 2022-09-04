using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buildersoft.Andy.X.Core.Extensions.Authorization
{
    public static class AuthorizationValidationExtensions
    {
        public static bool ValidateTenantToken(this ITenantStateService tenantStateService, ICoreRepository coreRepository, string tenant, string token, bool isConsumer = true)
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');

            var key = Guid.Parse(credentials.FirstOrDefault());
            var encryptedSecret = credentials.LastOrDefault();

            var tenantDetails = coreRepository.GetTenant(tenant);
            var tenantSettings = coreRepository.GetTenantSettings(tenantDetails.Id);
            if (tenantSettings.IsAuthorizationEnabled != true)
                return true;


            var tenantToken = coreRepository.GetTenantToken(key);
            if (tenantToken == null)
                return false;

            if (tenantToken.IsActive != true)
                return false;

            if (tenantToken.ExpireDate < DateTimeOffset.UtcNow)
            {
                tenantToken.IsActive = false;

                // Revoke token.
                coreRepository.EditTenantToken(tenantToken);

                return false;
            }

            if (tenantToken.Secret != encryptedSecret)
                return false;

            var roles = tenantToken.Roles;

            if (isConsumer == true)
                if (roles.Contains(TenantTokenRole.Consume) != true)
                    return false;

            if (isConsumer != true)
                if (roles.Contains(TenantTokenRole.Produce) != true)
                    return false;

            return true;
        }

        public static bool ValidateProductToken(this ITenantStateService tenantStateService, ICoreRepository coreRepository, long tenantId, string product, string token, bool isConsumer = true)
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');

            var key = Guid.Parse(credentials.FirstOrDefault());
            var encryptedSecret = credentials.LastOrDefault();

            var productDetails = coreRepository.GetProduct(tenantId, product);
            var productSettings = coreRepository.GetProductSettings(productDetails.Id);
            if (productSettings.IsAuthorizationEnabled != true)
                return true;


            var productToken = coreRepository.GetProductToken(key);
            if (productToken == null)
                return false;

            if (productToken.IsActive != true)
                return false;

            if (productToken.ExpireDate < DateTimeOffset.UtcNow)
            {
                productToken.IsActive = false;

                // Revoke token.
                coreRepository.EditProductToken(productToken);

                return false;
            }

            if (productToken.Secret != encryptedSecret)
                return false;

            var roles = productToken.Roles;

            if (isConsumer == true)
                if (roles.Contains(ProductTokenRole.Consume) != true)
                    return false;

            if (isConsumer != true)
                if (roles.Contains(ProductTokenRole.Produce) != true)
                    return false;

            return true;
        }

        public static bool ValidateComponentToken(this ITenantStateService tenantStateService, ICoreRepository coreRepository, long productId, string component, string token, string subscriptionProducerName, bool isConsumer = true)
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');

            var key = Guid.Parse(credentials.FirstOrDefault());
            var encryptedSecret = credentials.LastOrDefault();

            // tenantId = -1, this property is not being used in GetComponent
            var componentDetails = coreRepository.GetComponent(-1, productId, component);
            var componentSettings = coreRepository.GetComponentSettings(componentDetails.Id);
            if (componentSettings.IsAuthorizationEnabled != true)
                return true;


            var componentToken = coreRepository.GetComponentToken(key);
            if (componentToken == null)
                return false;

            if (componentToken.IsActive != true)
                return false;

            if (componentToken.ExpireDate < DateTimeOffset.UtcNow)
            {
                componentToken.IsActive = false;

                // Revoke token.
                coreRepository.EditComponentToken(componentToken);

                return false;
            }

            if (componentToken.Secret != encryptedSecret)
                return false;

            var roles = componentToken.Roles;

            if (isConsumer == true)
                if (roles.Contains(ComponentTokenRole.Consume) != true)
                    return false;

            if (isConsumer != true)
                if (roles.Contains(ComponentTokenRole.Produce) != true)
                    return false;

            if (componentToken.IssuedFor != subscriptionProducerName)
                return false;

            return true;
        }
    }
}
