using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost("tenants")]
        public ActionResult<string> AddTenant(string tenantName, [FromBody] TenantSettings tenantSettings)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} POST '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"POST '{HttpContext.Request.Path}' is called");

            return Ok(tenantName);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("tenants/{tenantName}")]
        public ActionResult<string> UpdateTenant(string tenantName, [FromBody] TenantSettings tenantSettings)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} PATCH '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"PATCH '{HttpContext.Request.Path}' is called");

            return Ok(tenantName);
        }
    }
}
