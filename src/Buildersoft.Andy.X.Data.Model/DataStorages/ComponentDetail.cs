using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.DataStorages
{
    public class ComponentDetail
    {
        public Guid ComponentId { get; set; }
        public string TenantName { get; set; }
        public string ProductName { get; set; }
        public string ComponentName { get; set; }

        public string ComponentDescription { get; set; }
        public bool ComponentStatus { get; set; }
    }
}
