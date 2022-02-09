using Buildersoft.Andy.X.Core.Abstractions.Repositories.Producers;
using Buildersoft.Andy.X.Model.Producers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1/producers")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    public class ProducersController : ControllerBase
    {
        private readonly ILogger<ProducersController> _logger;
        private readonly IProducerHubRepository _producerHubRepository;

        public ProducersController(ILogger<ProducersController> logger, IProducerHubRepository producerHubRepository)
        {
            _logger = logger;
            _producerHubRepository = producerHubRepository;
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

            var consumers = _producerHubRepository.GetAllProducers();

            return Ok(consumers);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{connectionId}")]
        public ActionResult<Producer> GetConsumer(string connectionId)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");


            var consumer = _producerHubRepository.GetProducerById(connectionId);
            if (consumer == null)
                return NotFound();

            return Ok(consumer);
        }
    }
}
