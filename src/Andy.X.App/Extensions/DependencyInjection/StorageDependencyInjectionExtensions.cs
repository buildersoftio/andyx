using Buildersoft.Andy.X.Core.Abstractions.Factories.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Core.Factories.Storages;
using Buildersoft.Andy.X.Router.Repositories.Storages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class StorageDependencyInjectionExtensions
    {
        public static void AddStorageFactoryMethods(this IServiceCollection services)
        {
            services.AddSingleton<IStorageFactory, StorageFactory>();
            services.AddSingleton<IAgentFactory, AgentFactory>();
        }

        public static void AddStorageRepository(this IServiceCollection services)
        {
            services.AddSingleton<IStorageHubRepository, StorageHubRepository>();
        }
    }
}
