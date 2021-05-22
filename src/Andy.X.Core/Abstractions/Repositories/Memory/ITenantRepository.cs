using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory
{
    public interface ITenantRepository
    {
        bool AddTenant(string tenantName, Tenant tenant);
        bool AddProduct(string tenant, string productName, Product product);
        bool AddComponent(string tenant, string product, string componentName, Component component);


        Tenant GetTenant(string tenant);
        Product GetProduct(string tenant, string product);
        Component GetComponent(string tenant, string product, string component);


        ConcurrentDictionary<string, Tenant> GetTenants();
        ConcurrentDictionary<string, Product> GetProducts(string tenant);
        ConcurrentDictionary<string, Component> GetComponents(string tenant, string product);
    }
}
