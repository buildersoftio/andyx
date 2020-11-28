using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Authentication;
using Buildersoft.Andy.X.Data.Model.Authorization;
using Buildersoft.Andy.X.Utilities.Authentication.Jwt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Authorization
{
    public interface IAuthorizationLogic
    {
        JwtToken ValidateAndBuildToken(AuthorizationTenantRequest authorizationTenantRequest, Tenant tenantDetails);
    }
}
