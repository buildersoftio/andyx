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
using Buildersoft.Andy.X.Model.Subscriptions;
using Subscription = Buildersoft.Andy.X.Model.Entities.Core.Subscriptions.Subscription;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/subscriptions")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ILogger<SubscriptionsController> _logger;
        private readonly ICoreRepository _coreRepository;
        private readonly ICoreService _coreService;
        private readonly ITenantStateService _tenantStateService;
        private readonly ITenantFactory _tenantFactory;
        private readonly ISubscriptionFactory _subscriptionFactory;


        public SubscriptionsController(ILogger<SubscriptionsController> logger,
            ICoreRepository coreRepository,
            ICoreService coreService,
            ITenantStateService tenantStateService,
            ITenantFactory tenantFactory,
            ISubscriptionFactory subscriptionFactory)
        {
            _logger = logger;
            _coreRepository = coreRepository;
            _coreService = coreService;
            _tenantStateService = tenantStateService;
            _tenantFactory = tenantFactory;
            _subscriptionFactory = subscriptionFactory;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<List<string>> GetSubscriptions(string tenant, string product, string component, string topic)
        {
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

            var subscriptions = _coreRepository
                .GetSubscriptions(topicDetails.Id)
                .Select(x => new { Name = x.Name, Type = x.SubscriptionType });

            return Ok(subscriptions);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{subscription}")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<Subscription> GetSubscription(string tenant, string product, string component, string topic, string subscription)
        {
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

            var subscriptionDetails = _coreRepository.GetSubscription(topicDetails.Id, subscription);
            if (subscriptionDetails is null)
                return NotFound($"Subscription {subscription} does not exists in {tenant}/{product}/{component}/{topic}");

            return Ok(subscriptionDetails);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{subscription}")]
        [Authorize(Roles = "admin")]
        public ActionResult<Subscription> CreateSubscription(string tenant,
            string product,
            string component,
            string topic,
            string subscription,
            [FromQuery] SubscriptionType subscriptionType,
            [FromQuery] SubscriptionMode subscriptionMode,
            [FromQuery] InitialPosition initialPosition)
        {
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

            var subscriptionDetails = _coreRepository.GetSubscription(topicDetails.Id, subscription);
            if (subscriptionDetails is not null)
                return NotFound($"Subscription {subscription} already exists");

            var isCreated = _coreService.CreateSubscription(tenant, product, component, topic, subscription, subscriptionType, subscriptionMode, initialPosition);
            if (isCreated == true)
            {
                _tenantStateService.AddSubscriptionConfiguration(tenant, product, component, topic, subscription,
                    _subscriptionFactory.CreateSubscription(tenant, product, component, topic, subscription, subscriptionType, subscriptionMode, initialPosition));

                return Ok($"Subscription {subscription} has been created");
            }

            return BadRequest("Something went wrong, subscription couldnot be created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{subscription}")]
        [Authorize(Roles = "admin")]
        public ActionResult<Subscription> CreateSubscription(string tenant, string product, string component, string topic, string subscription)
        {
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

            var subscriptionDetails = _coreRepository.GetSubscription(topicDetails.Id, subscription);
            if (subscriptionDetails is null)
                return NotFound($"Subscription {subscription} does not exists in {tenant}/{product}/{component}/{topic}");

            var isDeleted = _coreService.DeleteSubscription(tenant, product, component, topic, subscription);
            if (isDeleted == true)
            {
                return Ok($"Subscription has been deleted, this is async process, it will take some time to start reflecting");
            }

            return BadRequest("Something went wrong, subscription couldnot be deleted");
        }
    }
}
