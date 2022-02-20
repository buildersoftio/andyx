using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    [Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly ILogger<TenantsController> _logger;
        private readonly ITenantService _tenantService;

        public TenantsController(ILogger<TenantsController> logger, ITenantService tenantService)
        {
            _logger = logger;
            _tenantService = tenantService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("tenants")]
        public ActionResult<List<string>> GetTenants()
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var tenants = _tenantService.GetTenantsName();

            return Ok(tenants);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("tenants/{tenantName}")]
        public ActionResult<TenantConfiguration> GetTenant(string tenantName)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");


            var tenant = _tenantService.GetTenant(tenantName);
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("tenants/{tenantName}")]
        public ActionResult<string> AddTenant(string tenantName, [FromBody] TenantSettings tenantSettings)
        {
            tenantName = tenantName.ToLower().Replace(" ", string.Empty);
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} POST '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"POST '{HttpContext.Request.Path}' is called");

            if (_tenantService.CreateTenant(tenantName, tenantSettings) != true)
                return BadRequest("Couldn't add new tenant, or tenant already exists");

            return Ok($"Tenant {tenantName} has been created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("tenants/{tenantName}")]
        public ActionResult<string> UpdateTenant(string tenantName, [FromBody] TenantSettings tenantSettings)
        {
            tenantName = tenantName.ToLower().Replace(" ", string.Empty);

            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} PATCH '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"PATCH '{HttpContext.Request.Path}' is called");



            return Ok(tenantName);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("tenants/{tenantName}/tokens")]
        public ActionResult<string> AddTenantToken(string tenantName, [FromBody] DateTime expireDate)
        {
            tenantName = tenantName.ToLower().Replace(" ", string.Empty);

            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} POST '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"POST '{HttpContext.Request.Path}' is called");
            var token = _tenantService.AddToken(tenantName, expireDate);
            if (token != null)
                return Ok($"Token '{token}' has been created for tenant '{tenantName}'");

            return BadRequest("Something went wrong, try to create Token one more time");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("tenants/{tenantName}/tokens")]
        public ActionResult<List<TenantToken>> GetTenantTokens(string tenantName)
        {
            tenantName = tenantName.ToLower().Replace(" ", string.Empty);

            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");
            var tokens = _tenantService.GetTokens(tenantName);
            if (tokens != null)
                return Ok(tokens);

            return NotFound($"There is no tenant with name '{tenantName}'");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("tenants/{tenantName}/tokens/{token}/revoke")]
        public ActionResult<string> DeleteToken(string tenantName, string token)
        {
            tenantName = tenantName.ToLower().Replace(" ", string.Empty);

            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");
            var isTokenRevoked = _tenantService.RevokeToken(tenantName, token);
            if (isTokenRevoked == true)
                return Ok($"Token '{token}' has been revoked");

            return BadRequest($"Token '{token}' has not been revoked, or it doesnot exists");
        }
    }
}
