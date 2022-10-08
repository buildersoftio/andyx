using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Extensions;
using Buildersoft.Andy.X.Model.Clusters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Security.Claims;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/clusters")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    public class ClustersController : ControllerBase
    {
        private readonly ILogger<ClustersController> _logger;
        private readonly IClusterService _tenantService;
        private readonly IClusterRepository _clusterRepository;

        public ClustersController(ILogger<ClustersController> logger, 
            IClusterService tenantService,
            IClusterRepository clusterRepository)
        {
            _logger = logger;
            _tenantService = tenantService;
            _clusterRepository = clusterRepository;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<Cluster> GetClusters()
        {
            _logger.LogApiCallFrom(HttpContext);


            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var clusters = _tenantService.GetCluster();

            return Ok(clusters);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("nodes/current")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<Replica> GetCurrentNode()
        {
            _logger.LogApiCallFrom(HttpContext);


            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var clusters = _clusterRepository.GetCurrentReplica();

            return Ok(clusters);
        }
    }
}
