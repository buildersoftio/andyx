using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Tenants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Tenants
{
    public class TenantLogic : ITenantLogic
    {
        private readonly ITenantRepository _tenantRepository;
        public TenantLogic(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public TenantLogic(StorageMemoryRepository memoryRepository)
        {
            _tenantRepository = new TenantMemoryRepository(memoryRepository);
        }

        public Tenant CreateTenant(string tenantName)
        {
            Tenant tenant = new Tenant()
            {
                Name = tenantName,
                Description = tenantName,
                Signature = new Signature() { SecurityKey = Guid.NewGuid().ToString(), DigitalSignature = "KWDjwhAndyjp360qwetM2DFS43BuilderSoft" }
            };

            if (_tenantRepository.Add(tenant))
                return tenant;
            return null;
        }

        public List<Tenant> GetAllTenants()
        {
            return _tenantRepository.GetAllAsList();
        }

        public Tenant GetTenant(string tenantName)
        {
            return _tenantRepository.Get(tenantName);
        }
    }
}
