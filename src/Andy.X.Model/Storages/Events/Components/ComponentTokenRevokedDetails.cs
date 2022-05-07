using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Storages.Events.Components
{
    public class ComponentTokenRevokedDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }

        public string Token { get; set; }
        public List<string> StoragesAlreadySent { get; set; }

        public ComponentTokenRevokedDetails()
        {
            StoragesAlreadySent = new List<string>();
        }
    }
}
