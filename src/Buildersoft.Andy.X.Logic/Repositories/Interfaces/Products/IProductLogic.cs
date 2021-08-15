using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Products
{
    public interface IProductLogic
    {
        Product CreateProduct(string productName);
        Product GetProduct(string productName);
        List<Product> GetAllProducts();
    }
}
