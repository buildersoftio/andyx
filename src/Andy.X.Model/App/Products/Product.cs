using Buildersoft.Andy.X.Model.App.Components;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Model.App.Products
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ConcurrentDictionary<string, Component> Components { get; set; }

        public Product()
        {
            Components = new ConcurrentDictionary<string, Component>();
        }
    }
}
