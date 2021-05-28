using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Products
{
    public class ProductCreatedDetails
    {
        public string Tenant { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
