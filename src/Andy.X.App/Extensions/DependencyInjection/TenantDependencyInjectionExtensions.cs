using Buildersoft.Andy.X.Core.Abstractions.Repositories;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Repositories.App;
using Buildersoft.Andy.X.Core.Services.App;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class TenantDependencyInjectionExtensions
    {
        public static void AddTenantMemoryService(this IServiceCollection services)
        {
            services.AddSingleton<ITenantStateService, TenantStateService>();
        }

        public static void AddTenantMemoryRepository(this IServiceCollection services)
        {
            services.AddSingleton<ITenantStateRepository, TenantStateRepository>();
        }

        public static void UseTenantMemoryRepository(this IApplicationBuilder builder, IServiceProvider serviceProvider)
        {
            var tenantRepo = serviceProvider.GetRequiredService<ITenantStateService>();
        }
    }
}
