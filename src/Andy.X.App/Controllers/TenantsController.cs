using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v3/tenants")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
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
        [Authorize(Roles = "admin,readonly")]
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
        [Authorize(Roles = "admin,readonly")]
        public ActionResult<Tenant> GetTenant(string tenant)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            return Ok(tenantDetails);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{tenant}/activate")]
        [Authorize(Roles = "admin")]
        public ActionResult<Tenant> ActiveTenant(string tenant)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            if (_coreService.ActivateTenant(tenant))
                return Ok(_coreRepository.GetTenant(tenant));

            return BadRequest("Tenant couldn't activate at this moment");
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{tenant}/deactivate")]
        [Authorize(Roles = "admin")]
        public ActionResult<Tenant> DeactivateTenant(string tenant)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            if (_coreService.DeactivateTenant(tenant))
                return Ok(_coreRepository.GetTenant(tenant));

            return BadRequest("Tenant couldn't deactivate at this moment");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{tenant}/delete")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> DeleteTenant(string tenant)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var settings = _coreRepository.GetTenantSettings(tenantDetails.Id);
            if (_coreService.DeleteTenant(tenant))
                return Ok("Tenant is marked for deletion, this is an async process, andy will disconnect connections to this tenant, if you try to create a new tenant with the same name it may not work for now");

            return BadRequest("Tenant couldn't delete at this moment");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{tenant}/settings")]
        [Authorize(Roles = "admin,readonly")]
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
        [Authorize(Roles = "admin")]
        public ActionResult<string> UpdateTenantSettings(string tenant, [FromBody] TenantSettings tenantSettings)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            bool isUpdated = _coreService
                .UpdateTenantSettings(tenant, tenantSettings.IsProductAutomaticCreationAllowed, tenantSettings.IsEncryptionEnabled, tenantSettings.IsAuthorizationEnabled);
            if (isUpdated)
                return Ok("Settings have been updated, tenant in the cluster is marked to refresh settings, this may take a while");

            return BadRequest();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{tenant}")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> CreateTenant(string tenant, [FromBody] TenantSettings tenantSettings)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is not null)
                return BadRequest($"Tenant {tenant} already exists in the cluster");

            bool isTenantCreated = _coreService.CreateTenant(tenant, tenantSettings.IsProductAutomaticCreationAllowed, tenantSettings.IsEncryptionEnabled, tenantSettings.IsAuthorizationEnabled);
            if (isTenantCreated)
            {
                // update in memory

                _tenantStateService.AddTenant(tenant, _tenantFactory.CreateTenant(tenant, tenantSettings.IsEncryptionEnabled, tenantSettings.IsProductAutomaticCreationAllowed, tenantSettings.IsAuthorizationEnabled));
                var tenantName = tenant;
                return Ok("Tenant has been created");
            }

            return BadRequest("Something went wrong! Try again.");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{tenant}/tokens")]
        [Authorize(Roles = "admin,readonly")]
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
        [Authorize(Roles = "admin,readonly")]
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
        [Authorize(Roles = "admin")]
        public ActionResult<string> RevokeTenantToken(string tenant, Guid key)
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
        [Authorize(Roles = "admin")]
        public ActionResult<string> PostTenantTokens(string tenant, [FromBody] TenantToken tenantToken)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            bool tenantTokenCreated = _coreService.CreateTenantToken(tenant, tenantToken.Description, tenantToken.ExpireDate, tenantToken.Roles, out Guid key, out string secret);
            if (tenantTokenCreated)
                return Ok(new { Key = key, Secret = secret });

            return BadRequest("Something went wrong, token couldnot be created");
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{tenant}/retentions")]
        [Authorize(Roles = "admin,readonly")]
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
        [Authorize(Roles = "admin")]
        public ActionResult<string> PostTenantRetentions(string tenant, [FromBody] TenantRetention tenantRetention)
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
        [Authorize(Roles = "admin")]
        public ActionResult<string> PutTenantRetentions(string tenant, long id, [FromBody] TenantRetention tenantRetention)
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{tenant}/retentions/id")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> DeleteTenantRetentions(string tenant, long id)
        {
            var tenantDetails = _coreRepository.GetTenant(tenant);
            if (tenantDetails is null)
                return NotFound($"Tenant {tenant} does not exists in this cluster");

            var retentionDeleted = _coreService.DeleteTenantRetention(tenant, id);
            if (retentionDeleted)
            {
                return Ok("Tenant Retention has been deleted, this is async process, it will take some time to start reflecting");
            }

            return BadRequest("Update of tenant retention deleted couldnot happend, please try again");
        }
    }
}
