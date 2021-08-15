using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Authorization;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Authorization;
using Buildersoft.Andy.X.Logic.Interfaces.Tenants;
using Buildersoft.Andy.X.Logic.Tenants;
using Buildersoft.Andy.X.Utilities.Authentication.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1/tenants")]
    [ApiController]
    [RequireHttps]
    public class AuthorizationController : ControllerBase
    {
        private readonly ITenantLogic _tenantLogic;
        private readonly IAuthorizationLogic _authorizationLogic;

        public AuthorizationController(StorageMemoryRepository memoryRepository, IAuthorizationLogic authorizationLogic)
        {
            _tenantLogic = new TenantLogic(memoryRepository);
            _authorizationLogic = authorizationLogic;
        }

        [HttpPost("{tenantName}/access_token")]
        public ActionResult<JwtToken> GenerateTenantAccessToken(string tenantName, [FromBody] AuthorizationTenantRequest authorizationTenantRequest)
        {
            var tenantDetails = _tenantLogic.GetTenant(tenantName);
            if (tenantDetails == null)
                return NotFound("TENANT_NOT_FOUND");

            var token = _authorizationLogic.ValidateAndBuildToken(authorizationTenantRequest, tenantDetails);
            if (token == null)
                return Forbid($"Credentials are not valid for this tenant {tenantName}");

            return Ok(token);
        }
    }
}
