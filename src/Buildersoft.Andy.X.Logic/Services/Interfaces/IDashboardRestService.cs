using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Services.Interfaces
{
    public interface IDashboardRestService
    {
        List<Tenant> GetTenants();
        ICollection<string> GetTenantNames();
        ICollection<string> GetProductNames(string tenant);
        List<Product> GetProducts(string tenant);
        ICollection<string> GetComponentNames(string tenant, string product);
        List<Component> GetComponents(string tenant, string product);
        ICollection<string> GetBookNames(string tenant, string product, string component);
        List<Book> GetBooks(string tenant, string product, string component);
        ICollection<string> GetReaderNames(string tenant, string product, string component, string books);
        List<Reader> GetReaders(string tenant, string product, string component, string books);
    }
}
