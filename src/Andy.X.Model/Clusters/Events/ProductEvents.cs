using Buildersoft.Andy.X.Model.Entities.Core.Products;
using System;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class ProductCreatedArgs
    {
        public string Tenant { get; set; }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class ProductUpdatedArgs
    {
        public string Tenant { get; set; }

        public string Name { get; set; }
        public ProductSettings ProductSettings { get; set; }
    }
    public class ProductDeletedArgs
    {
        public string Tenant { get; set; }
        public string Name { get; set; }
    }

    public class ProductTokenCreatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public Entities.Core.Products.ProductToken ProductToken { get; set; }
    }
    public class ProductTokenRevokedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public Guid Key { get; set; }
    }
    public class ProductTokenDeletedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public Guid Key { get; set; }
    }

    public class ProductRetentionCreatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public Entities.Core.Products.ProductRetention ProductRetention { get; set; }
    }
    public class ProductRetentionUpdatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public Entities.Core.Products.ProductRetention ProductRetention { get; set; }
    }
    public class ProductRetentionDeletedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public Entities.Core.Products.ProductRetention ProductRetention { get; set; }
    }
}
