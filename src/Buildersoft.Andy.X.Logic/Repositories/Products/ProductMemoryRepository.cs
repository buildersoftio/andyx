using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Products;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Products
{
    public class ProductMemoryRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<string, Product> _products;
        public ProductMemoryRepository(ConcurrentDictionary<string, Product> products)
        {
            _products = products;
        }

        public bool Add(Product product)
        {
            return _products.TryAdd(product.Name, product);
        }

        public Product Get(string productName)
        {
            if (_products.ContainsKey(productName))
                return _products[productName];
            return null;
        }

        public List<Product> GetAll()
        {
            return _products.Values.ToList();
        }

        public ConcurrentDictionary<string, Product> GetProductsDictionary()
        {
            return _products;
        }
    }
}
