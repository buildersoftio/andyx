using System;

namespace Buildersoft.Andy.X.Model.App.Components
{
    public class ComponentToken
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // TOKEN will be generated from andyx-cli and it be added manually via tenants.json
        public string Token { get; set; }

        public bool IsActive { get; set; }

        public bool CanConsume { get; set; }
        public bool CanProduce { get; set; }

        public DateTime ExpireDate { get; set; }
        public string IssuedFor { get; set; }
        public DateTime IssuedDate { get; set; }
    }
}
