using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Storages.Requests.Components
{
    public class RevokeComponentTokenDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }

        public string Token { get; set; }
        public List<string> StoragesAlreadySent { get; set; }

        public RevokeComponentTokenDetails()
        {
            StoragesAlreadySent = new List<string>();
        }
    }
}
