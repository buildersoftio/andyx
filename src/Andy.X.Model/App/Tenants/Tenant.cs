using Buildersoft.Andy.X.Model.App.Products;
using System;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Model.App.Tenants
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ConcurrentDictionary<string, Product> Products { get; set; }
        public TenantSettings Settings{ get; set; }

        public Tenant()
        {
            Products = new ConcurrentDictionary<string, Product>();
            Settings = new TenantSettings();
        }
    }
}
