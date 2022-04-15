using Buildersoft.Andy.X.Core.Abstractions.Services.Api.Lineage;
using Buildersoft.Andy.X.Model.App.Lineage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1/tenants/{tenantName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    // [Authorize]
    public class StreamLineageController : ControllerBase
    {
        private readonly ILogger<StreamLineageController> _logger;
        private readonly IStreamLineageService _streamLineageService;

        public StreamLineageController(ILogger<StreamLineageController> logger, IStreamLineageService streamLineageService)
        {
            _logger = logger;
            _streamLineageService = streamLineageService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("lineage")]
        public ActionResult<List<StreamLineage>> GetTenantLineage(string tenantName)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var streamLineages = _streamLineageService.GetStreamLineages(tenantName);

            return Ok(streamLineages);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("products/{productName}/lineage")]
        public ActionResult<List<StreamLineage>> GetProductLineage(string tenantName, string productName)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var streamLineages = _streamLineageService.GetStreamLineages(tenantName, productName);

            return Ok(streamLineages);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("products/{productName}/components/{componentName}/lineage")]
        public ActionResult<List<StreamLineage>> GetComponentLineage(string tenantName, string productName, string componentName)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var streamLineages = _streamLineageService.GetStreamLineages(tenantName, productName, componentName);

            return Ok(streamLineages);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("products/{productName}/components/{componentName}/topics/{topicName}/lineage")]
        public ActionResult<StreamLineage> GetTopicLineage(string tenantName, string productName, string componentName, string topicName)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var streamLineage = _streamLineageService.GetStreamLineage(tenantName, productName, componentName, topicName);

            return Ok(streamLineage);
        }
    }
}
