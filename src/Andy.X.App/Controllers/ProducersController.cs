using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Buildersoft.Andy.X.Model.Entities.Core.Producers;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Extensions;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/producers")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    public class ProducersController : ControllerBase
    {
        private readonly ILogger<ProducersController> _logger;
        private readonly ICoreRepository _coreRepository;
        private readonly ICoreService _coreService;

        public ProducersController(ILogger<ProducersController> logger,
            ICoreRepository coreRepository,
            ICoreService coreService)
        {
            _logger = logger;
            _coreRepository = coreRepository;
            _coreService = coreService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<List<string>> GetProducers(string tenant, string product, string component, string topic)
        {
            _logger.LogApiCallFrom(HttpContext);

            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var componentDetails = _coreRepository.GetComponent(tenantDetails.Id, productDetails.Id, component);
            if (componentDetails is null)
                return NotFound($"Component {component} does not exists in {tenant}/{product}");

            var topicDetails = _coreRepository.GetTopic(componentDetails.Id, topic);
            if (topicDetails is null)
                return NotFound($"Topic {topic} does not exists in {tenant}/{product}/{component}");

            var producers = _coreRepository
                .GetProducers(topicDetails.Id)
                .Select(x => x.Name);

            return Ok(producers);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{producer}")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<Producer> GetProducer(string tenant, string product, string component, string topic, string producer)
        {
            _logger.LogApiCallFrom(HttpContext);

            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var componentDetails = _coreRepository.GetComponent(tenantDetails.Id, productDetails.Id, component);
            if (componentDetails is null)
                return NotFound($"Component {component} does not exists in {tenant}/{product}");

            var topicDetails = _coreRepository.GetTopic(componentDetails.Id, topic);
            if (topicDetails is null)
                return NotFound($"Topic {topic} does not exists in {tenant}/{product}/{component}");

            var producerDetails = _coreRepository.GetProducer(topicDetails.Id, producer);
            if (producerDetails is null)
                return NotFound($"Producer {producer} does not exists in {tenant}/{product}/{component}/{topic}");

            return Ok(producerDetails);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{producer}")]
        [Authorize(Roles = "admin")]
        public ActionResult<Subscription> CreateProducer(string tenant, string product, string component,
            string topic, string producer, [FromQuery] string description, [FromQuery] ProducerInstanceType instanceType)
        {
            _logger.LogApiCallFrom(HttpContext);

            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var componentDetails = _coreRepository.GetComponent(tenantDetails.Id, productDetails.Id, component);
            if (componentDetails is null)
                return NotFound($"Component {component} does not exists in {tenant}/{product}");

            var topicDetails = _coreRepository.GetTopic(componentDetails.Id, topic);
            if (topicDetails is null)
                return NotFound($"Topic {topic} does not exists in {tenant}/{product}/{component}");

            var producerDetails = _coreRepository.GetProducer(topicDetails.Id, producer);
            if (producerDetails is not null)
                return NotFound($"Producer {producer} already exists in {tenant}/{product}/{component}/{topic}");

            var isCreated = _coreService.CreateProducer(tenant, product, component, topic, producer, description, instanceType);
            if (isCreated == true)
            {
                return Ok($"Producer {producer} has been created");
            }

            return BadRequest("Something went wrong, producer couldnot be created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{producer}")]
        [Authorize(Roles = "admin")]
        public ActionResult<Subscription> DeleteSubscription(string tenant, string product, string component, string topic, string producer)
        {
            _logger.LogApiCallFrom(HttpContext);

            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var componentDetails = _coreRepository.GetComponent(tenantDetails.Id, productDetails.Id, component);
            if (componentDetails is null)
                return NotFound($"Component {component} does not exists in {tenant}/{product}");

            var topicDetails = _coreRepository.GetTopic(componentDetails.Id, topic);
            if (topicDetails is null)
                return NotFound($"Topic {topic} does not exists in {tenant}/{product}/{component}");

            var producerDetails = _coreRepository.GetProducer(topicDetails.Id, producer);
            if (producerDetails is null)
                return NotFound($"Producer {producer} does not exists in {tenant}/{product}/{component}/{topic}");

            var isDeleted = _coreService.DeleteProducer(tenant, product, component, topic, producer);
            if (isDeleted == true)
            {
                return Ok($"Producer is marked for deletion, this is async process, it will take some time to start reflecting");
            }

            return BadRequest("Something went wrong, producer couldnot be deleted");
        }

    }
}
