using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Tenants
{
    public interface ITenantRepository
    {
        bool Add(Tenant tenant);
        Tenant TryAddOrGet(Tenant tenant);
        Tenant Get(string tenantName);
        List<Tenant> GetAllAsList();
        ConcurrentDictionary<string, Tenant> GetAllAsDictionary();
    }
}
