using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Services.Interfaces
{
    public interface IDashboardRestService
    {
        ICollection<string> GetTenantNames();
        Tenant GetTenantDetails(string tenant);
        ICollection<string> GetProductNames(string tenant);
        Product GetProductDetails(string tenant, string product);
        ICollection<string> GetComponentNames(string tenant, string product);
        Component GetComponentDetails(string tenant, string product, string component);
        ICollection<string> GetBookNames(string tenant, string product, string component);
        Book GetBookDetails(string tenant, string product, string component, string book);
        ICollection<string> GetReaderNames(string tenant, string product, string component, string books);
        Reader GetReaderDetails(string tenant, string product, string component, string books, string reader);
    }
}
