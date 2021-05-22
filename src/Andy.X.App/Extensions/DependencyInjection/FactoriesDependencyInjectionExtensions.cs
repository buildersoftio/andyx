using Buildersoft.Andy.X.Core.Abstractions.Factories.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Factories.Storages;
using Buildersoft.Andy.X.Core.Factories.Tenants;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class FactoriesDependencyInjectionExtensions
    {
        public static void AddStorageFactoryMethods(this IServiceCollection services)
        {
            services.AddSingleton<IStorageFactory, StorageFactory>();
            services.AddSingleton<IAgentFactory, AgentFactory>();
        }

        public static void AddAppFactoryMethods(this IServiceCollection services)
        {
            services.AddSingleton<ITenantFactory, TenantFactory>();
        }
    }
}
