using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Components;
using Buildersoft.Andy.X.Logic.Interfaces.Components;
using Buildersoft.Andy.X.Router.Services.DataStorages;
using Buildersoft.Andy.X.Utilities.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [RequireHttps]
    [Authorize(TenantOnly = true)]
    public class ComponentsController : ControllerBase
    {
        private IComponentLogic _componentLogic;
        private readonly StorageMemoryRepository _memoryRepository;
        private readonly ComponentService _componentService;

        public ComponentsController(StorageMemoryRepository memoryRepository, ComponentService componentService)
        {
            _memoryRepository = memoryRepository;
            _componentService = componentService;
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components")]
        public ActionResult<List<Component>> GetAllComponents(string tenantName, string productName)
        {
            _componentLogic = new ComponentLogic(
                _memoryRepository.GetComponents(tenantName, productName));

            return Ok(_componentLogic.GetAllComponents());
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}")]
        public ActionResult<Component> GetComponent(string tenantName, string productName, string componentName)
        {
            _componentLogic = new ComponentLogic(
                _memoryRepository.GetComponents(tenantName, productName));

            Component component = _componentLogic.GetComponent(componentName);
            if (component != null)
                return Ok(component);
            return NotFound("COMPONENT_NOT_FOUND");
        }

        [HttpPost("tenants/{tenantName}/products/{productName}/components/{componentName}")]
        public async Task<ActionResult<Component>> CreateComponent(string tenantName, string productName, string componentName)
        {
            _componentLogic = new ComponentLogic(
                _memoryRepository.GetComponents(tenantName, productName));

            if (_componentLogic.GetComponent(componentName) != null)
                return BadRequest("COMPONENT_EXISTS");

            Component component = _componentLogic.CreateComponent(componentName);
            if (component != null)
            {
                await _componentService.CreateComponentAsync(new Data.Model.DataStorages.ComponentDetail()
                {
                    ComponentId = component.Id,
                    TenantName = tenantName,
                    ProductName = productName,
                    ComponentName = componentName,
                    ComponentDescription = component.Description,
                    ComponentStatus = component.Status
                });

                return Ok(component);
            }

            return BadRequest("COMPONENT_NOT_CREATED");
        }
    }
}