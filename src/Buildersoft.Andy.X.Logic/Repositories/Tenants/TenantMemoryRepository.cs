using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Tenants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Tenants
{
    public class TenantMemoryRepository : ITenantRepository
    {
        private readonly ConcurrentDictionary<string, Tenant> _tenants;
        public TenantMemoryRepository(StorageMemoryRepository memoryRepository)
        {
            _tenants = memoryRepository.GetTenants();
        }

        public bool Add(Tenant tenant)
        {
            return _tenants.TryAdd(tenant.Name, tenant);
        }

        public Tenant Get(string tenantName)
        {
            if (_tenants.ContainsKey(tenantName))
                return _tenants[tenantName];
            return null;
        }

        public List<Tenant> GetAllAsList()
        {
            return _tenants.Values.ToList();
        }

        public ConcurrentDictionary<string, Tenant> GetAllAsDictionary()
        {
            return _tenants;
        }

        public Tenant TryAddOrGet(Tenant tenant)
        {
            return _tenants.GetOrAdd(tenant.Name, tenant);
        }
    }
}
