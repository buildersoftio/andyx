using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Products;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Products
{
    public class ProductLogic : IProductLogic
    {
        private readonly IProductRepository _productRepository;
        public ProductLogic(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public ProductLogic(ConcurrentDictionary<string, Product> products)
        {
            _productRepository = new ProductMemoryRepository(products);
        }

        public Product CreateProduct(string productName)
        {
            Product product = new Product() { Name = productName };
            if (_productRepository.Add(product))
                return product;
            return null;
        }

        public List<Product> GetAllProducts()
        {
            return _productRepository.GetAll();
        }

        public Product GetProduct(string productName)
        {
            return _productRepository.Get(productName);
        }
    }
}
