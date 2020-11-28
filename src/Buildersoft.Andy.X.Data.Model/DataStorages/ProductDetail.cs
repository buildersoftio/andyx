using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.DataStorages
{
   public class ProductDetail
    {
        public Guid ProductId { get; set; }
        public string TenantName { get; set; }
        public string ProductName{ get; set; }

        // TODO... Add properties for this Product
    }
}
