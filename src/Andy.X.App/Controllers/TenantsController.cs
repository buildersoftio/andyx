using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/tenants")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    //[Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly ILogger<TenantsController> _logger;
        private readonly ICoreRepository _coreRepository;
        private readonly ICoreService _coreService;
        private readonly ITenantStateService _tenantStateService;
        private readonly ITenantFactory _tenantFactory;

        public TenantsController(ILogger<TenantsController> logger,
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
        public ActionResult<List<string>> GetTenants()
        {
            var tenants = _coreRepository
                .GetTenants()
                .Select(x => x.Name);

            return Ok(tenants);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{tenant}")]
        public ActionResult<Tenant> GetTenant(string tenant)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            return Ok(tenantDetails);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{tenant}/settings")]
        public ActionResult<TenantSettings> GetTenantSettings(string tenant)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var settings = _coreRepository.GetTenantSettings(tenantDetails.Id);

            return Ok(settings);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{tenant}/settings")]
        public ActionResult<string> UpdateTenantSettings(string tenant, [FromBody] TenantSettings tenantSettings)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            bool isUpdated = _coreService
                .UpdateTenantSettings(tenant, tenantSettings.IsProductAutomaticCreation, tenantSettings.IsEncryptionEnabled, tenantSettings.IsAuthorizationEnabled);
            if (isUpdated)
                return Ok("Settings have been updated, tenant in the cluster is marked to refresh settings, this may take a while");

            return BadRequest();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{tenant}")]
        public ActionResult<string> CreateTenant(string tenant, [FromBody] TenantSettings tenantSettings)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is not null)
                return BadRequest($"Tenant {tenant} already exists in the cluster");

            bool isTenantCreated = _coreService.CreateTenant(tenant, tenantSettings.IsProductAutomaticCreation, tenantSettings.IsEncryptionEnabled, tenantSettings.IsAuthorizationEnabled);
            if (isTenantCreated)
            {
                // update in memory

                _tenantStateService.AddTenant(tenant, _tenantFactory.CreateTenant(tenant, tenantSettings.IsEncryptionEnabled, tenantSettings.IsProductAutomaticCreation, tenantSettings.IsAuthorizationEnabled));
                var tenantName = tenant;
                return Ok("Tenant has been created");
            }

            return BadRequest("Something went wrong! Try again.");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{tenant}/tokens")]
        public ActionResult<List<string>> GetTenantTokens(string tenant)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var tokens = _coreRepository.GetTenantToken(tenantDetails.Id).Select(x => new
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
        [HttpGet("{tenant}/tokens/{key}")]
        public ActionResult<TenantToken> GetTenantToken(string tenant, Guid key)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var token = _coreRepository.GetTenantToken(key);
            if (token is null)
                return NotFound("Tenant Token does not exists");

            return Ok(token);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{tenant}/tokens/{key}/revoke")]
        public ActionResult<TenantToken> RevokeTenantToken(string tenant, Guid key)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var token = _coreRepository.GetTenantToken(key);
            if (token is null)
                return NotFound("Tenant Token does not exists");

            if (_coreService.RevokeTenantToken(tenant, key))
                return Ok("Token has been revoked");

            return BadRequest("Token couldnot revoke at this moment, try again");

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{tenant}/tokens")]
        public ActionResult<List<string>> PostTenantTokens(string tenant, [FromBody] TenantToken tenantToken)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            bool tenantTokenCreated = _coreService.CreateTenantToken(tenant, tenantToken.Description, tenantToken.ExpireDate, tenantToken.Roles, out Guid key, out string secret);
            if (tenantTokenCreated)
                return Ok(new { Key = key, Secret = secret });

            return BadRequest("Something went wrong, token has not been created");
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{tenant}/retentions")]
        public ActionResult<List<TenantRetention>> GetTenantRetentions(string tenant)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var retentions = _coreRepository.GetTenantRetentions(tenantDetails.Id);

            return Ok(retentions);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{tenant}/retentions")]
        public ActionResult<List<TenantRetention>> PostTenantRetentions(string tenant, [FromBody] TenantRetention tenantRetention)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var retentionCreated = _coreService.CreateTenantRetention(tenant, tenantRetention.Name, tenantRetention.Type, tenantRetention.TimeToLiveInMinutes);
            if (retentionCreated)
            {
                return Ok("Tenant Retention has been created, this is async process, it will take some time to start reflecting");
            }

            return BadRequest("Retention with the same type exists already, please update the stored one");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{tenant}/retentions/id")]
        public ActionResult<List<TenantRetention>> PostTenantRetentions(string tenant, long id, [FromBody] TenantRetention tenantRetention)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var retentionUpdated = _coreService.UpdateTenantRetention(tenant, id, tenantRetention.Name, tenantRetention.TimeToLiveInMinutes);
            if (retentionUpdated)
            {
                return Ok("Tenant Retention has been updated, this is async process, it will take some time to start reflecting");
            }

            return BadRequest("Update of Tenant Retention couldnot happend, please try again");
        }
    }
}
