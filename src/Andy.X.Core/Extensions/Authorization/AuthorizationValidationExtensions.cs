using Buildersoft.Andy.X.Core.Abstractions.Services;
using System;

namespace Buildersoft.Andy.X.Core.Extensions.Authorization
{
    public static class AuthorizationValidationExtensions
    {
        public static bool ValidateTenantToken(this ITenantStateService inMemoryRepo, string tenant, string token)
        {
            var t = inMemoryRepo.GetTenant(tenant);

            if (t.Settings.IsAuthorizationEnabled != true)
                return true;

            //var tenantToken = inMemoryRepo.GetTenantToken(tenant, token);
            //if (tenantToken == null)
            //    return false;

            //if (tenantToken.IsActive != true)
            //    return false;

            //if (tenantToken.ExpireDate < DateTime.Now)
            //    return false;

            //// TBD should we validate token with build-in authorization from .NET?
            //if (tenantToken.Token != token)
            //    return false;

            return true;
        }

        public static bool ValidateComponentToken(this ITenantStateService inMemoryRepo, string tenant, string product, string componet, string token, string consumerProducerName, bool isConsumer = true)
        {
            var c = inMemoryRepo.GetComponent(tenant, product, componet);

            if (c.Settings.IsAuthorizationEnabled != true)
                return true;

            //var componentToken = inMemoryRepo.GetComponentToken(tenant, product, componet, token);
            //if (componentToken == null)
            //    return false;

            //if (componentToken.IsActive != true)
            //    return false;

            //if (componentToken.ExpireDate < DateTime.Now)
            //    return false;

            //if (isConsumer == true)
            //{
            //    if (componentToken.CanConsume != true)
            //        return false;
            //}
            //else
            //{
            //    if (componentToken.CanProduce != true)
            //        return false;
            //}

            //if (componentToken.IssuedFor != consumerProducerName)
            //    return false;

            //// TBD should we validate token with build-in authorization from .NET?
            //if (componentToken.Token != token)
            //    return false;

            return true;
        }
    }
}
