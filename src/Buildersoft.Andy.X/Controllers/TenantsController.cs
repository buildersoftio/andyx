using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Interfaces.Tenants;
using Buildersoft.Andy.X.Logic.Tenants;
using Buildersoft.Andy.X.Router.Services.DataStorages;
using Buildersoft.Andy.X.Utilities.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [RequireHttps]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantLogic _tenantLogic;
        private readonly TenantService _tenantService;
        public TenantsController(StorageMemoryRepository memoryRepository, TenantService tenantService)
        {
            _tenantService = tenantService;
            _tenantLogic = new TenantLogic(memoryRepository);
        }

        [HttpGet]
        public ActionResult<List<Tenant>> GetAllTenants()
        {
            return Ok(_tenantLogic.GetAllTenants());
        }

        [HttpGet("{tenantName}")]
        [Authorize(TenantOnly = true)]
        public ActionResult<List<Tenant>> GetTenant(string tenantName)
        {
            string tenantFromToken = HttpContext.Items["Tenant"].ToString();
            if (tenantName.ToString() != tenantFromToken)
                return Forbid($"You do not have access to read this tenant {tenantName}");

            Tenant tenant = _tenantLogic.GetTenant(tenantName);
            if (tenant != null)
            {
                if (tenant.Status == true)
                    return Ok(tenant);
                return BadRequest($"Tenant {tenantName} is inactive");
            }

            return NotFound("TENANT_NOT_FOUND");
        }

        [HttpPost("{tenantName}")]
        public async Task<ActionResult<Tenant>> CreateTenant(string tenantName)
        {
            if (_tenantLogic.GetTenant(tenantName) != null)
                return BadRequest("TENANT_EXISTS");

            Tenant tenant = _tenantLogic.CreateTenant(tenantName);
            if (tenant != null)
            {

                await _tenantService.CreateTenantAsync(new Data.Model.DataStorages.TenantDetail()
                {
                    TenantName = tenantName,
                    Encryption = tenant.GetEncryption(),
                    Signature = tenant.GetSignature(),
                    TenantId = tenant.Id,
                    TenantDescription = tenant.Description,
                    TenantStatus = tenant.Status
                });

                return Ok(new
                {
                    tenantDetail = tenant,
                    Signature = tenant.GetSignature()
                });
            }

            return BadRequest("TENANT_NOT_CREATED");
        }
    }
}