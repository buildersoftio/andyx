using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Authentication;
using Buildersoft.Andy.X.Data.Model.Authorization;
using Buildersoft.Andy.X.Utilities.Authentication.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Authorization
{

    public class AuthorizationLogic : IAuthorizationLogic
    {
        private JwtConfiguration _jwtConfiguration;
        public AuthorizationLogic(IOptions<JwtConfiguration> jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration.Value;
        }

        public JwtToken ValidateAndBuildToken(AuthorizationTenantRequest authorizationTenantRequest, Tenant tenantDetails)
        {
            if(authorizationTenantRequest.TenantId == tenantDetails.Id)
                if(authorizationTenantRequest.SecurityKey == tenantDetails.GetSignature().SecurityKey)
                {
                    var token = new JwtTokenBuilder()
                        .AddSecurityKey(JwtSecurityKey.Create(tenantDetails.GetSignature().DigitalSignature))
                        .AddSubject($"{tenantDetails.Name}")
                        .AddAudience(_jwtConfiguration.Audience)
                        .AddIssuer(_jwtConfiguration.Issuer)
                        .AddClaim("TenantId", tenantDetails.Id.ToString())
                        .AddClaim("Tenant", tenantDetails.Name)
                        .AddClaim("SecurityKey", authorizationTenantRequest.SecurityKey)
                        .AddExpiry(Convert.ToInt32(new TimeSpan(60, 0, 0, 0).TotalMinutes))
                        .Build();

                    return token;
                }

            return null;
        }
    }
}
