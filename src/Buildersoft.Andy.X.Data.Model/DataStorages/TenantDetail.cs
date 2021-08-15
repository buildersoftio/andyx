using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.DataStorages
{
    public class TenantDetail
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantDescription { get; set; }
        public bool TenantStatus { get; set; }
        public Encryption Encryption { get; set; }
        public Signature Signature { get; set; }
    }
}
