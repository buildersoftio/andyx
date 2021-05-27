using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Tenants
{
    public class TenantCreatedDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DigitalSignature { get; set; }
    }
}
