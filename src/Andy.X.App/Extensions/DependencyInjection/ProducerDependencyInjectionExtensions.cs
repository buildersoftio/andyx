using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Producers;
using Buildersoft.Andy.X.Router.Repositories.Producers;
using Buildersoft.Andy.X.Router.Services.Producers;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class ProducerDependencyInjectionExtensions
    {
        public static void AddProducerRepository(this IServiceCollection services)
        {
            services.AddSingleton<IProducerHubRepository, ProducerHubRepository>();
        }

        public static void AddProducerHubService(this IServiceCollection services)
        {
            services.AddSingleton<IProducerHubService, ProducerHubService>();
        }
    }
}
