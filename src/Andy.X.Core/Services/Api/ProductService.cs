using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Model.App.Products;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Buildersoft.Andy.X.Core.Services.Api
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly ITenantRepository _tenantRepository;

        public ProductService(ILogger<ProductService> logger, ITenantRepository tenantRepository)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
        }

        public Product GetProduct(string tenantName, string productName)
        {
            return _tenantRepository.GetProduct(tenantName, productName);
        }

        public List<Product> GetProducts(string tenantName)
        {
            var result = new List<Product>();
            var products = _tenantRepository.GetProducts(tenantName).Select(x => x.Value).ToList();

            products.ForEach(x =>
            {
                result.Add(new Product() { Id = x.Id, Name = x.Name });
            });

            return result;
        }
    }
}
