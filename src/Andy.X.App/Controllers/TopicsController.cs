using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Collections.Generic;
using System.Linq;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Buildersoft.Andy.X.Extensions;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Core.Abstractions.Repositories;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/tenants/{tenant}/products/{product}/components/{component}/topics")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly ILogger<TopicsController> _logger;
        private readonly ICoreRepository _coreRepository;
        private readonly ICoreService _coreService;
        private readonly ITenantStateService _tenantStateService;
        private readonly ITenantFactory _tenantFactory;
        private readonly ITenantStateRepository _tenantStateRepository;

        public TopicsController(ILogger<TopicsController> logger,
            ICoreRepository coreRepository,
            ICoreService coreService,
            ITenantStateService tenantStateService,
            ITenantFactory tenantFactory,
            ITenantStateRepository tenantStateRepository)
        {
            _logger = logger;
            _coreRepository = coreRepository;
            _coreService = coreService;
            _tenantStateService = tenantStateService;
            _tenantFactory = tenantFactory;
            this._tenantStateRepository = tenantStateRepository;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<List<string>> GetTopics(string tenant, string product, string component)
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

            var topics = _coreRepository
                .GetTopics(componentDetails.Id)
                .Select(x => x.Name);

            return Ok(topics);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{topic}")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<Model.Entities.Core.Topics.Topic> GetTopic(string tenant, string product, string component, string topic)
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

            return Ok(topicDetails);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{topic}")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> CreateTopic(string tenant, string product, string component, string topic, [FromQuery] string description, [FromBody] TopicSettings topicSettings)
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
            if (topicDetails is not null)
                return BadRequest($"Topic {topic} already exists");

            var isCreated = _coreService.CreateTopic(tenant, product, component, topic, description, topicSettings);
            if (isCreated == true)
            {
                _tenantStateService.AddTopic(tenant, product, component, topic, _tenantFactory.CreateTopic(topic, description), false);
                return Ok($"Topic {topic} has been created");
            }

            return BadRequest("Something went wrong, topic couldnot be created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{topic}")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> UpdateTopic(string tenant, string product, string component, string topic, [FromQuery] string description)
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

            var isUpdated = _coreService.UpdateTopic(tenant, product, component, topic, description);
            if (isUpdated == true)
            {
                return Ok($"Topic {topic} has been updated");
            }

            return BadRequest("Something went wrong, topic couldnot be updated");
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{topic}")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> DeleteTopic(string tenant, string product, string component, string topic)
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

            var isDeleted = _coreService.DeleteTopic(tenant, product, component, topic);
            if (isDeleted == true)
            {
                return Ok($"Topic has been deleted, this is async process, it will take some time to start reflecting");
            }

            return BadRequest("Something went wrong, topic couldnot be deleted");
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{topic}/settings")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<TopicSettings> GetTopicSettings(string tenant, string product, string component, string topic)
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

            var settings = _coreRepository.GetTopicSettings(topicDetails.Id);

            return Ok(settings);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{topic}/settings")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> UpdateTopicSettings(string tenant, string product, string component, string topic, [FromBody] TopicSettings topicSettings)
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

            var isUpdated = _coreService.UpdateTopicSettings(tenant, product, component, topic, topicSettings);
            if (isUpdated == true)
                return Ok("Topic settings have been updated, this topic is marked to refresh settings, this may take a while. It also can disconnect clients connected to this topic");


            return BadRequest("Something went wrong, topic settings couldnot be updated");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{topic}/state")]
        [Authorize(Roles = "admin")]
        public ActionResult<TopicStates> GetTopicState(string tenant, string product, string component, string topic)
        {
            _logger.LogApiCallFrom(HttpContext);

            var topicDetail = _tenantStateRepository.GetTopic(tenant, product, component, topic);
            if (topicDetail is null)
                return NotFound("Topic not found");

            return Ok(topicDetail.TopicStates);
        }
    }
}
