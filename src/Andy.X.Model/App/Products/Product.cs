using Buildersoft.Andy.X.Model.App.Components;
using System;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Model.App.Products
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ConcurrentDictionary<string, Component> Components{ get; set; }
        public Product()
        {
            Components = new ConcurrentDictionary<string, Component>();
        }
    }
}
