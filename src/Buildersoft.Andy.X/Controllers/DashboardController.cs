using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1/dashboard")]
    [ApiController]
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

        [HttpGet("tenants/{tenantName}")]
        public ActionResult<List<Tenant>> GetTenants(string tenantName)
        {
            return Ok(_dashboardService.GetTenantDetails(tenantName));
        }

        [HttpGet("tenants/{tenantName}/products")]
        public ActionResult<ICollection<string>> GetProductNames(string tenantName)
        {
            return Ok(_dashboardService.GetProductNames(tenantName));
        }

        [HttpGet("tenants/{tenantName}/products/{productName}")]
        public ActionResult<object> GetProductDetails(string tenantName, string productName)
        {
            return Ok(_dashboardService.GetProductDetails(tenantName, productName));
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components")]
        public ActionResult<ICollection<string>> GetComponentNames(string tenantName, string productName)
        {
            return Ok(_dashboardService.GetComponentNames(tenantName, productName));
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}")]
        public ActionResult<object> GetComponentDetails(string tenantName, string productName, string componentName)
        {
            return Ok(_dashboardService.GetComponentDetails(tenantName, productName, componentName));
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}/books")]
        public ActionResult<ICollection<string>> GetBookNames(string tenantName, string productName, string componentName)
        {
            return Ok(_dashboardService.GetBookNames(tenantName, productName, componentName));
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}")]
        public ActionResult<object> GetBookDetail(string tenantName, string productName, string componentName, string bookName)
        {
            return Ok(_dashboardService.GetBookDetails(tenantName, productName, componentName, bookName));
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/readers")]
        public ActionResult<ICollection<string>> GetReaderNames(string tenantName, string productName, string componentName, string bookName)
        {
            return Ok(_dashboardService.GetReaderNames(tenantName, productName, componentName, bookName));
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/readers/{readerName}")]
        public ActionResult<object> GetReaderDetails(string tenantName, string productName, string componentName, string bookName, string readerName)
        {
            return Ok(_dashboardService.GetReaderDetails(tenantName, productName, componentName, bookName, readerName));
        }
    }
}
