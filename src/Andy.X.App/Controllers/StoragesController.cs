using Buildersoft.Andy.X.Attributes;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Model.Storages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1/storages")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    [BasicAuthorize]
    public class StoragesController : ControllerBase
    {
        private readonly ILogger<StoragesController> _logger;
        private readonly IStorageHubRepository _storageHubRepository;

        public StoragesController(ILogger<StoragesController> logger, IStorageHubRepository storageHubRepository)
        {
            _logger = logger;
            _storageHubRepository = storageHubRepository;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("")]
        public ActionResult<List<string>> GetStorages()
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");

            var storages = _storageHubRepository.GetStorages().Keys.ToList();

            return Ok(storages);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{storageName}")]
        public ActionResult<Storage> GetStorage(string storageName)
        {
            var isFromCli = HttpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                _logger.LogInformation($"{isFromCli} GET '{HttpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"GET '{HttpContext.Request.Path}' is called");


            var storage = _storageHubRepository.GetStorageByName(storageName);
            if (storage == null)
                return NotFound();

            return Ok(storage);
        }
    }
}
