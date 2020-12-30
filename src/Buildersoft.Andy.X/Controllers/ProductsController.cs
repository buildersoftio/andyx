using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Interfaces.Products;
using Buildersoft.Andy.X.Logic.Products;
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
    public class ProductsController : ControllerBase
    {
        private IProductLogic _productLogic;
        private readonly StorageMemoryRepository _memoryRepository;
        private readonly ProductService _productService;
        public ProductsController(StorageMemoryRepository memoryRepository, ProductService productService)
        {
            _memoryRepository = memoryRepository;
            _productService = productService;
        }

        [HttpGet("tenants/{tenantName}/products")]
        public ActionResult<List<Product>> GetAllProducts(string tenantName)
        {
            _productLogic = new ProductLogic(_memoryRepository.GetProducts(tenantName));

            return Ok(_productLogic.GetAllProducts());
        }

        [HttpGet("tenants/{tenantName}/products/{productName}")]
        public ActionResult<Product> GetProduct(string tenantName, string productName)
        {
            _productLogic = new ProductLogic(_memoryRepository.GetProducts(tenantName));

            Product product = _productLogic.GetProduct(productName);
            if (product != null)
                return Ok(product);
            return NotFound("PRODUCT_NOT_FOUND");
        }

        [HttpPost("tenants/{tenantName}/products/{productName}")]
        public async Task<ActionResult<Product>> CreateProduct(string tenantName, string productName)
        {
            _productLogic = new ProductLogic(_memoryRepository.GetProducts(tenantName));

            if (_productLogic.GetProduct(productName) != null)
                return BadRequest("PRODUCT_EXISTS");

            Product product = _productLogic.CreateProduct(productName);
            if (product != null)
            {
                await _productService.CreateProductAsync(new Data.Model.DataStorages.ProductDetail()
                {
                    ProductId = product.Id,
                    TenantName = tenantName,
                    ProductName = productName,
                    ProductDescription = product.Description,
                    ProductStatus = product.Status
                });
                return Ok(product);
            }

            return BadRequest("PRODUCT_NOT_CREATED");
        }
    }
}