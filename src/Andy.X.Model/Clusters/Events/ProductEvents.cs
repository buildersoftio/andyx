using System;
using System.Collections.Generic;

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
        public string Description { get; set; }
    }

    public class ProductDeletedArgs
    {
        public string Tenant { get; set; }
        public string Name { get; set; }
    }
}
