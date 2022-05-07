using Buildersoft.Andy.X.Model.App.Products;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Api
{
    public interface IProductService
    {
        Product GetProduct(string tenantName, string productName);
        List<Product> GetProducts(string tenantName);

        bool AddProduct(string tenant, string productName);
    }
}
