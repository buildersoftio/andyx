using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Extensions;
using Buildersoft.Andy.X.Model.Producers;
using Buildersoft.Andy.X.Model.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/activities")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly ILogger<ActivitiesController> _logger;
        private readonly ICoreRepository _coreRepository;
        private readonly ICoreService _coreService;
        private readonly ISubscriptionHubRepository _subscriptionHubRepository;
        private readonly IProducerHubRepository _producerHubRepository;

        public ActivitiesController(ILogger<ActivitiesController> logger,
            ICoreRepository coreRepository,
            ICoreService coreService,
            ISubscriptionHubRepository subscriptionHubRepository,
            IProducerHubRepository producerHubRepository)
        {
            _logger = logger;
            _coreRepository = coreRepository;
            _coreService = coreService;
            _subscriptionHubRepository = subscriptionHubRepository;
            _producerHubRepository = producerHubRepository;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("tenants/count")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<long> GetTenantsCount()
        {
            _logger.LogApiCallFrom(HttpContext);

            var result = _coreRepository.GetTenantsCount();
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("products/count")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<long> GetProductsCount()
        {
            _logger.LogApiCallFrom(HttpContext);

            var result = _coreRepository.GetProductsCount();
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("components/count")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<long> GetComponentsCount()
        {
            _logger.LogApiCallFrom(HttpContext);

            var result = _coreRepository.GetComponentsCount();
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("topics/count")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<long> GetTopicsCount()
        {
            _logger.LogApiCallFrom(HttpContext);

            var result = _coreRepository.GetTopicsCount();
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("subscriptions/count")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<long> GetSubscriptionsCount()
        {
            _logger.LogApiCallFrom(HttpContext);

            var result = _coreRepository.GetSubscriptionsCount();
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("producers/count")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<long> GetProducersCount()
        {
            _logger.LogApiCallFrom(HttpContext);

            var result = _coreRepository.GetProducersCount();
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("subscriptions")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<List<SubscriptionActivity>> GetSubscriptions()
        {
            _logger.LogApiCallFrom(HttpContext);

            var activeSubscription = _subscriptionHubRepository.GetAllSubscriptionActivities();
            return Ok(activeSubscription);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("producers")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<List<ProducerActivity>> GetProducers()
        {
            _logger.LogApiCallFrom(HttpContext);

            var activeProducer = _producerHubRepository.GetAllProducerActivities();
            return Ok(activeProducer);
        }
    }
}
