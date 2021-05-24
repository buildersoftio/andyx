using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.App.Repositories.Memory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class TenantDependencyInjectionExtensions
    {
        public static void AddTenantMemoryRepository(this IServiceCollection services)
        {
            services.AddSingleton<ITenantRepository, TenantMemoryRepository>();
        }

        public static void UseTenantMemoryRepository(this IApplicationBuilder builder, IServiceProvider serviceProvider)
        {
            var tenantRepo = serviceProvider.GetRequiredService<ITenantRepository>();
        }
    }
}
