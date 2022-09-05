using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using Buildersoft.Andy.X.Core.Factories.Tenants;
using Buildersoft.Andy.X.Core.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Services.App;
using Buildersoft.Andy.X.Core.Services.CoreState;
using Serilog.Core;
using System.Collections.Generic;
using System.Linq;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using System;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/tenants/{tenant}/products/{product}/components")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    //[Authorize]
    public class ComponentsController : ControllerBase
    {
        private readonly ILogger<ComponentsController> _logger;
        private readonly ICoreRepository _coreRepository;
        private readonly ICoreService _coreService;
        private readonly ITenantStateService _tenantStateService;
        private readonly ITenantFactory _tenantFactory;
        public ComponentsController(ILogger<ComponentsController> logger,
            ICoreRepository coreRepository,
            ICoreService coreService,
            ITenantStateService tenantStateService,
            ITenantFactory tenantFactory)
        {
            _logger = logger;
            _coreRepository = coreRepository;
            _coreService = coreService;
            _tenantStateService = tenantStateService;
            _tenantFactory = tenantFactory;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("")]
        public ActionResult<List<string>> GetComponents(string tenant, string product)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var components = _coreRepository
                .GetComponents(productDetails.Id)
                .Select(x => x.Name);

            return Ok(components);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{component}")]
        public ActionResult<Component> GetComponent(string tenant, string product, string component)
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

            return Ok(componentDetails);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{component}")]
        public ActionResult<string> CreateComponent(string tenant, string product, string component, [FromQuery] string description, [FromBody] ComponentSettings componentSettings)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var componentDetails = _coreRepository.GetComponent(tenantDetails.Id, productDetails.Id, component);
            if (componentDetails is not null)
                return BadRequest($"Product already exists");

            var isCreated = _coreService.CreateComponent(tenant, product, component, description, componentSettings.IsTopicAutomaticCreationAllowed, componentSettings.IsSchemaValidationEnabled, componentSettings.IsAuthorizationEnabled, componentSettings.IsSubscriptionAutomaticCreationAllowed);
            if (isCreated == true)
            {
                _tenantStateService.AddComponent(tenant, product, component, _tenantFactory.CreateComponent(component, description, componentSettings));
                return Ok("Component has been created");
            }

            return BadRequest("Something went wrong, component couldnot be created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{component}")]
        public ActionResult<string> UpdateComponent(string tenant, string product, string component, [FromQuery] string description)
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

            var isUpdated = _coreService.UpdateComponent(tenant, product, component, description);
            if (isUpdated == true)
            {
                return Ok("Component has been updated");
            }

            return BadRequest("Something went wrong, component couldnot be updated");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{component}")]
        public ActionResult<string> DeleteComponent(string tenant, string product, string component)
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

            var isDeleted = _coreService.DeleteComponent(tenant, product, component);
            if (isDeleted == true)
                return Ok("Component has been deleted, this is async process, it will take some time to start reflecting");

            return BadRequest("Something went wrong, component couldnot be deleted");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{component}/settings")]
        public ActionResult<ComponentSettings> GetComponentSettings(string tenant, string product, string component)
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

            var settings = _coreRepository.GetComponentSettings(componentDetails.Id);

            return Ok(settings);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{component}/settings")]
        public ActionResult<string> UpdateComponentSettings(string tenant, string product, string component, [FromBody] ComponentSettings componentSettings)
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

            var isUpdated = _coreService.UpdateComponentSettings(tenant, product, component, componentSettings.IsTopicAutomaticCreationAllowed, componentSettings.IsSchemaValidationEnabled, componentSettings.IsAuthorizationEnabled, componentSettings.IsSubscriptionAutomaticCreationAllowed);
            if (isUpdated == true)
                return Ok("Component settings have been updated, product in the tenant is marked to refresh settings, this may take a while");


            return BadRequest("Something went wrong, component settings couldnot be updated");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{component}/tokens")]
        public ActionResult<string> CreateComponentToken(string tenant, string product, string component, [FromBody] ComponentToken componentToken)
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

            var isCreated = _coreService.CreateComponentToken(tenant, product, component, componentToken.Description, componentToken.IssuedFor, componentToken.ExpireDate, componentToken.Roles, out Guid key, out string secret);
            if (isCreated == true)
                return Ok(new { Key = key, Secret = secret });

            return BadRequest("Something went wrong, token couldnot be created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{component}/tokens/{key}/revoke")]
        public ActionResult<string> CreateComponentToken(string tenant, string product, string component, Guid key)
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

            var isRevoked = _coreService.RevokeComponentToken(tenant, product, component, key);
            if (isRevoked == true)
                return Ok("Token has been revoked");

            return BadRequest("Token couldnot revoke at this moment, try again");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{component}/tokens")]
        public ActionResult<List<ComponentToken>> GetComponentTokens(string tenant, string product, string component)
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

            var tokens = _coreRepository.GetComponentToken(componentDetails.Id).Select(x => new
            {
                Key = x.Id,
                Description = x.Description,
                ExpireDate = x.ExpireDate,
                IsActive = x.IsActive
            });

            return Ok(tokens);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{component}/tokens/{key}")]
        public ActionResult<ComponentToken> GetComponentToken(string tenant, string product, string component, Guid key)
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

            var token = _coreRepository.GetComponentToken(key);
            if (token is null)
                return NotFound("Token with this key do not exists");

            return Ok(token);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{component}/retentions")]
        public ActionResult<List<ComponentRetention>> GetComponentRetentions(string tenant, string product, string component)
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

            var retentions = _coreRepository.GetComponentRetentions(componentDetails.Id);

            return Ok(retentions);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{component}/retentions")]
        public ActionResult<string> AddComponentRetention(string tenant, string product, string component, [FromBody] ComponentRetention componentRetention)
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

            var isCreated = _coreService.CreateComponentRetention(tenant, product, component,componentRetention.Name, componentRetention.Type, componentRetention.TimeToLiveInMinutes);
            if (isCreated)
                return Ok("Component retention has been created, this is async process, it will take some time to start reflecting");

            return BadRequest("Retention with the same type exists already, please update the stored one");

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{component}/retentions/{id}")]
        public ActionResult<string> UpdateComponentRetention(string tenant, string product, string component, long id,[FromBody] ComponentRetention componentRetention)
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

            var isUpdated = _coreService.UpdateComponentRetention(tenant, product, component, id, componentRetention.Name, componentRetention.TimeToLiveInMinutes);
            if (isUpdated)
                return Ok("Component retention has been updated, this is async process, it will take some time to start reflecting");

            return BadRequest("Update of Component retention couldnot happend, please try again");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{component}/retentions/{id}")]
        public ActionResult<string> DeleteComponentRetention(string tenant, string product, string component, long id)
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

            var isDeleted = _coreService.DeleteComponentRetention(tenant, product, component, id);
            if (isDeleted)
                return Ok("Component retention has been deleted, this is async process, it will take some time to start reflecting");

            return BadRequest("Delete of Component retention couldnot happend, please try again");
        }

    }
}
