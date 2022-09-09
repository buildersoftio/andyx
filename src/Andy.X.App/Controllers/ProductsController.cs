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
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/tenants/{tenant}/products")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ICoreRepository _coreRepository;
        private readonly ICoreService _coreService;
        private readonly ITenantStateService _tenantStateService;
        private readonly ITenantFactory _tenantFactory;

        public ProductsController(ILogger<ProductsController> logger,
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
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<List<string>> GetProducts(string tenant)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");


            var tenants = _coreRepository
                .GetProducts(tenantDetails.Id)
                .Select(x => x.Name);

            return Ok(tenants);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{product}")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<Product> GetProduct(string tenant, string product)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            return Ok(productDetails);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{product}")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> CreateProduct(string tenant, string product, [FromQuery] string description, [FromBody] ProductSettings productSettings)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is not null)
                return BadRequest($"Product already exists");

            var isCreated = _coreService.CreateProduct(tenant, product, description, productSettings.IsAuthorizationEnabled);
            if (isCreated == true)
            {
                _tenantStateService.AddProduct(tenant, product, _tenantFactory.CreateProduct(product, description));
                return Ok("Product has been created");
            }

            return BadRequest("Something went wrong, product couldnot be created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{product}")]
        [Authorize(Roles = "admin")]
        public ActionResult<Product> UpdateProduct(string tenant, string product, [FromQuery] string description)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return BadRequest($"Product {product} does not exists in this tenant");

            bool isUpdated = _coreService.UpdateProduct(tenant, product, description);
            if (isUpdated == true)
            {
                return Ok(_coreRepository.GetProduct(tenantDetails.Id, product));
            }

            return BadRequest("Something went wrong, product couldnot be created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{product}/delete")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> DeleteProduct(string tenant, string product)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return BadRequest($"Product {product} does not exists in this tenant");

            bool isDeleted = _coreService.DeleteProduct(tenant, product);
            if (isDeleted == true)
                return Ok("Product is marked for deletion, this is an async process, andy will disconnect connections to this product, if you try to create a new product with the same name it may not work for now");


            return BadRequest("Something went wrong, Product couldnot be created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{product}/settings")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<ProductSettings> GetProductSettings(string tenant, string product)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var productSettings = _coreRepository.GetProductSettings(productDetails.Id);

            return Ok(productSettings);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{product}/settings")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> UpdateProductSettings(string tenant, string product, [FromBody] ProductSettings productSettings)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return BadRequest($"Product {product} does not exists in this tenant");

            bool isUpdated = _coreService.UpdateProductSettings(tenant, product, productSettings.IsAuthorizationEnabled);
            if (isUpdated == true)
                return Ok("Product settings have been updated, product in the tenant is marked to refresh settings, this may take a while");

            return BadRequest("Something went wrong, product settings couldnot be updated");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{product}/tokens")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> CreateProductToken(string tenant, string product, [FromBody] ProductToken productToken)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var isCreated = _coreService.CreateProductToken(tenant, product, productToken.Description, productToken.ExpireDate, productToken.Roles, out Guid key, out string secret);
            if (isCreated == true)
                return Ok(new { Key = key, Secret = secret });

            return BadRequest("Something went wrong, token couldnot be created");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{product}/tokens/{key}/revoke")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> RevokeProductToken(string tenant, string product, Guid key)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var isRevoked = _coreService.RevokeProductToken(tenant, product, key);
            if (isRevoked == true)
                return Ok("Token has been revoked");

            return BadRequest("Token couldnot revoke at this moment, try again");
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{product}/tokens")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<List<ProductToken>> GetProductTokens(string tenant, string product)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var tokens = _coreRepository.GetProductToken(productDetails.Id).Select(x => new
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
        [HttpGet("{product}/tokens/{key}")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<ProductToken> GetProductToken(string tenant, Guid key, string product)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var token = _coreRepository.GetProductToken(key);
            if (token is null)
                return NotFound("Token with this key do not exists");
            return Ok(token);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{product}/retentions")]
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<List<ProductRetention>> GetProductRetentions(string tenant, string product)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var retentions = _coreRepository.GetProductRetentions(productDetails.Id);

            return Ok(retentions);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{product}/retentions")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> AddProductRetention(string tenant, string product, [FromBody] ProductRetention productRetention)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var isCreated = _coreService.CreateProductRetention(tenant, product, productRetention.Name, productRetention.Type, productRetention.TimeToLiveInMinutes);
            if (isCreated)
                return Ok("Product retention has been created, this is async process, it will take some time to start reflecting");

            return BadRequest("Retention with the same type exists already, please update the stored one");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{product}/retentions/{id}")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> UpdateProductRetention(string tenant, string product, long id, [FromBody] ProductRetention productRetention)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var isUpdated = _coreService.UpdateProductRetention(tenant, product, id, productRetention.Name, productRetention.TimeToLiveInMinutes);
            if (isUpdated)
                return Ok("Product retention has been updated, this is async process, it will take some time to start reflecting");

            return BadRequest("Update of Product retention couldnot happend, please try again");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{product}/retentions/{id}")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> DeleteProductRetention(string tenant, string product, long id)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var productDetails = _coreRepository.GetProduct(tenantDetails.Id, product);
            if (productDetails is null)
                return NotFound($"Product {product} does not exists in {tenant}");

            var isDeleted = _coreService.DeleteProductRetention(tenant, product, id);
            if (isDeleted)
                return Ok("Product retention has been deleted, this is async process, it will take some time to start reflecting");

            return BadRequest("Delete of Product retention couldnot happend, please try again");
        }
    }
}
