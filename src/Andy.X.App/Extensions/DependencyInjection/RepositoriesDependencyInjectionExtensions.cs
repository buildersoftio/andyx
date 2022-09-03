using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Repositories.CoreState;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class RepositoriesDependencyInjectionExtensions
    {
        public static void AddCoreRepository(this IServiceCollection services)
        {
            services.AddSingleton<ICoreRepository, CoreRepository>();
        }
    }
}
