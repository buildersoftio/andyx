using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Authentication;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Interfaces.Tenants;
using Buildersoft.Andy.X.Logic.Tenants;
using Buildersoft.Andy.X.Utilities.Authentication.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly ITenantLogic _tenantLogic;
        private readonly TokenValidationParameters tokenValidationParameters;

        public AuthorizationMiddleware(RequestDelegate next, IOptions<JwtConfiguration> jwtConfiguration, StorageMemoryRepository memoryRepository)
        {
            _next = next;
            _jwtConfiguration = jwtConfiguration.Value;
            _tenantLogic = new TenantLogic(memoryRepository);

            tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                // For now we will not validate Lifetime, but tokens will have 60 days validity
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtConfiguration.Issuer,
                ValidAudience = _jwtConfiguration.Audience
            };
        }

        public async Task Invoke(HttpContext context)
        {
            var tenantName = context.Request.Headers["x-andy-x-tenant"].ToString();
            if (tenantName == "")
            {
                context.Items["IsTenantValidated"] = false;
                
                var dataStorageName = context.Request.Headers["x-andyx-datastorage"].ToString();
                if (dataStorageName != "")
                {
                    // Check datastorage service
                    // TODO... implement token validation for DataStorages
                }
            }
            else
                CheckTenantAccessToken(tenantName, context);

            await _next(context);
        }

        private void CheckTenantAccessToken(string tenantName, HttpContext context)
        {
            var tenantDetails = _tenantLogic.GetTenant(tenantName);
            if (tenantDetails != null)
            {
                tokenValidationParameters.IssuerSigningKey = JwtSecurityKey.Create(tenantDetails.GetSignature().DigitalSignature);

                var tenantToken = context.Request.Headers["x-andy-x-tenant-Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (tenantToken != null)
                    context.Items["IsTenantValidated"] = ValidateTenantAccessToken(tenantToken, tenantDetails, context);
            }
            else
            {
                context.Items["IsTenantValidated"] = false;
            }
        }

        private bool ValidateTenantAccessToken(string tenantToken, Tenant tenantDetails, HttpContext context)
        {
            try
            {
                SecurityToken validatedToken;
                new JwtSecurityTokenHandler()
                  .ValidateToken(tenantToken, tokenValidationParameters, out validatedToken);

                JwtSecurityToken jwtToken = validatedToken as JwtSecurityToken;

                Claim tenantClaim = jwtToken.Claims.Where(claim => claim.Type == "Tenant").FirstOrDefault();
                context.Items["Tenant"] = tenantClaim.Value;
                var securityKeyFromToken = jwtToken.Claims.Where(claim => claim.Type == "SecurityKey").FirstOrDefault().Value;
                if (securityKeyFromToken != tenantDetails.GetSignature().SecurityKey)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
