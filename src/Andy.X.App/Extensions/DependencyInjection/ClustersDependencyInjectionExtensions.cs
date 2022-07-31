using Buildersoft.Andy.X.Core.Abstractions.Repositories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Core.Repositories;
using Buildersoft.Andy.X.Core.Services.Clusters;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class ClustersDependencyInjectionExtensions
    {
        public static void AddClusterRepository(this IServiceCollection services)
        {
            services.AddSingleton<IClusterRepository, ClusterMemoryRepository>();
        }

        public static void AddClusterService(this IServiceCollection services)
        {
            services.AddSingleton<IClusterService, ClusterService>();
        }
    }
}
