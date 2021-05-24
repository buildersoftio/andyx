using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Producers;
using Buildersoft.Andy.X.Router.Repositories.Consumers;
using Buildersoft.Andy.X.Router.Repositories.Producers;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class RepositoriesDependencyInjectionExtensions
    {
        public static void AddHubRepository(this IServiceCollection services)
        {
            services.AddSingleton<IProducerHubRepository, ProducerHubRepository>();
            services.AddSingleton<IConsumerHubRepository, ConsumerHubRepository>();
        }
    }
}
