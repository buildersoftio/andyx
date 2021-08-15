using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Products
{
    public interface IProductRepository
    {
        bool Add(Product product);
        Product Get(string productName);
        List<Product> GetAll();
        ConcurrentDictionary<string, Product> GetProductsDictionary();
    }
}
