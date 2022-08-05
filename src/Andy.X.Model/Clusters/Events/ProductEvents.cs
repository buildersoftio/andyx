using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class ProductCreatedArgs
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductOwner { get; set; }
        public List<string>? ProductTeam { get; set; }
        public string ProductContact { get; set; }
    }

    public class ProductUpdatedArgs
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductOwner { get; set; }
        public List<string>? ProductTeam { get; set; }
        public string ProductContact { get; set; }
    }

    public class ProductDeletedArgs
    {
        public string Name { get; set; }
    }
}
