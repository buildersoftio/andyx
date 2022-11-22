using Buildersoft.Andy.X.Core.Extensions;
using Buildersoft.Andy.X.Extensions;
using Buildersoft.Andy.X.Model.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/node")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly ILogger<NodeController> _logger;

        public NodeController(ILogger<NodeController> logger)
        {
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("version")]
        public ActionResult<ApplicationDetails> GetVersion()
        {
            _logger.LogApiCallFrom(HttpContext);

            return Ok(new ApplicationDetails()
            {
                Name = ApplicationProperties.Name,
                ShortName = ApplicationProperties.ShortName,
                Version = ApplicationProperties.Version
            });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("users/role")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<string> GetUserRole()
        {
            _logger.LogApiCallFrom(HttpContext);

            var role = HttpContext
                .User
                .Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault();

            return Ok(role.Value);
        }
    }
}
