using Buildersoft.Andy.X.Model.App.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.App.Products
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public string ProductOwner { get; set; }
        public List<string>? ProductTeam { get; set; }
        public string ProductContact { get; set; }

        public ConcurrentDictionary<string, Component> Components { get; set; }
        public Product()
        {
            Components = new ConcurrentDictionary<string, Component>();
            ProductTeam = new List<string>();
        }
    }
}
