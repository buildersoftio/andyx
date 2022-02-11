using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Model.App.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1/tenants/{tenantName}/products/{productName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    [Authorize]
    public class ComponentsController : ControllerBase
    {
        private readonly ILogger<ComponentsController> _logger;
        private readonly IComponentService _componentService;

        public ComponentsController(ILogger<ComponentsController> logger, IComponentService componentService)
        {
            _logger = logger;
            _componentService = componentService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("components")]
        public ActionResult<List<Component>> GetTenants(string tenantName, string productName)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var components = _componentService.GetComponents(tenantName, productName);

            return Ok(components);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("components/{componentName}")]
        public ActionResult<Component> GetComponent(string tenantName, string productName, string componentName)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var component = _componentService.GetComponent(tenantName, productName, componentName);
            if (component == null)
                return NotFound();

            return Ok(component);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("components/{componentName}/tokens")]
        public ActionResult<Component> PostComponentToken(string tenantName, string productName, string componentName, [FromBody] ComponentToken componentToken)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} POST '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"POST '{HttpContext.Request.Path}' is called");

            var token = _componentService.AddComponentToken(tenantName, productName, componentName, componentToken);
            if (token == null)
                return BadRequest("Something went wrong, try to create Token one more time");

            return Ok($"Token '{token}' has been created for component '{componentName}'");
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("components/{componentName}/tokens")]
        public ActionResult<List<ComponentToken>> GetTenantTokens(string tenantName, string productName, string componentName)
        {
            tenantName = tenantName.ToLower().Replace(" ", string.Empty);

            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");
            var tokens = _componentService.GetComponentTokens(tenantName, productName, componentName);
            if (tokens != null)
                return Ok(tokens);

            return NotFound($"There is no component with name '{componentName}' at tenant '{tenantName}'");
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("components/{componentName}/retention")]
        public ActionResult<Component> PostComponentRetentionPolicy(string tenantName, string productName, string componentName, [FromBody] ComponentRetention retention)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} POST '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"POST '{HttpContext.Request.Path}' is called");

            var retentionName = _componentService.AddRetentionPolicy(tenantName, productName, componentName, retention);
            if (retentionName == null)
                return BadRequest("Something went wrong, try to create Token one more time");

            return Ok($"Retention Policy '{retentionName}' has been modified for component '{componentName}'");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("components/{componentName}/retention")]
        public ActionResult<ComponentRetention> GetTenantRetention(string tenantName, string productName, string componentName)
        {
            tenantName = tenantName.ToLower().Replace(" ", string.Empty);

            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");
            var tokens = _componentService.GetRetentionPolicy(tenantName, productName, componentName);
            if (tokens != null)
                return Ok(tokens);

            return NotFound($"There is no retention policy created with name at component '{componentName}'");
        }
    }
}
