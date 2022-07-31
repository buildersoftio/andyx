using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1/consumers")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    [Authorize]
    public class ConsumersController : ControllerBase
    {
        private readonly ILogger<ConsumersController> _logger;
        private readonly ISubscriptionHubRepository _consumerHubRepository;

        public ConsumersController(ILogger<ConsumersController> logger, ISubscriptionHubRepository consumerHubRepository)
        {
            _logger = logger;
            _consumerHubRepository = consumerHubRepository;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("")]
        public ActionResult<List<string>> GetConsumers()
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var consumers = _consumerHubRepository.GetAllSubscriptionNames();

            return Ok(consumers);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{subscriptionName}")]
        public ActionResult<Subscription> GetSubscription(string subscriptionName)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");


            var consumer = _consumerHubRepository.GetSubscriptionById(subscriptionName);
            if (consumer == null)
                return NotFound();

            return Ok(consumer);
        }
    }
}
