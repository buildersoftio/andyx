using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1/dashboard")]
    [ApiController]
    [Authorize(Policy = "Internal")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRestService _dashboardService;

        public DashboardController(IDashboardRestService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("tenants")]
        public ActionResult<ICollection<string>> GetTenantNames()
        {
            return Ok(_dashboardService.GetTenantNames());
        }

        [HttpGet("tenants/detail")]
        public ActionResult<List<Tenant>> GetTenants()
        {
            return Ok(_dashboardService.GetTenants());
        }

        [HttpGet("tenants{tenantName}/products")]
        public ActionResult<ICollection<string>> GetProductNames(string tenantName)
        {
            return Ok(_dashboardService.GetProductNames(tenantName));
        }

        [HttpGet("tenants{tenantName}/products/detail")]
        public ActionResult<ICollection<string>> GetProducts(string tenantName)
        {
            return Ok(_dashboardService.GetProducts(tenantName));
        }

        [HttpGet("tenants{tenantName}/products/{productName}/components")]
        public ActionResult<ICollection<string>> GetComponentNames(string tenantName, string productName)
        {
            return Ok(_dashboardService.GetComponentNames(tenantName, productName));
        }

        [HttpGet("tenants{tenantName}/products/{productName}/components/detail")]
        public ActionResult<ICollection<string>> GetComponents(string tenantName, string productName)
        {
            return Ok(_dashboardService.GetComponents(tenantName, productName));
        }

        [HttpGet("tenants{tenantName}/products/{productName}/components/{componentName}/books")]
        public ActionResult<ICollection<string>> GetBookNames(string tenantName, string productName, string componentName)
        {
            return Ok(_dashboardService.GetBookNames(tenantName, productName, componentName));
        }

        [HttpGet("tenants{tenantName}/products/{productName}/components/{componentName}/detail")]
        public ActionResult<ICollection<string>> GetBooks(string tenantName, string productName, string componentName)
        {
            return Ok(_dashboardService.GetBooks(tenantName, productName, componentName));
        }

        [HttpGet("tenants{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/readers")]
        public ActionResult<ICollection<string>> GetReaderNames(string tenantName, string productName, string componentName, string bookName)
        {
            return Ok(_dashboardService.GetReaderNames(tenantName, productName, componentName, bookName));
        }

        [HttpGet("tenants{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/readers/detail")]
        public ActionResult<ICollection<string>> GetReaders(string tenantName, string productName, string componentName, string bookName)
        {
            return Ok(_dashboardService.GetReaders(tenantName, productName, componentName, bookName));
        }
    }
}
