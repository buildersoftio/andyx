using Buildersoft.Andy.X.Logic.Interfaces.Tenants;
using Buildersoft.Andy.X.Logic.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Extensions.DI
{
    public static class MemoryRepository
    {
        public static IServiceCollection AddTenantRepositories(this IServiceCollection services)
        {
            services.AddSingleton<ITenantRepository, TenantMemoryRepository>();

            return services;
        }
    }
}
