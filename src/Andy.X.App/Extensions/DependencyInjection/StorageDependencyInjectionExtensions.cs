using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Router.Repositories.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class StorageDependencyInjectionExtensions
    {
        public static void AddStorageRepository(this IServiceCollection services)
        {
            services.AddSingleton<IStorageHubRepository, StorageHubRepository>();
        }
    }
}
