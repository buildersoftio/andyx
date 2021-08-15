using Buildersoft.Andy.X.Core.Abstractions.Repositories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Consumers;
using Buildersoft.Andy.X.Router.Repositories.Consumers;
using Buildersoft.Andy.X.Router.Services.Consumers;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class ConsumerDependencyInjectionExtensions
    {
        public static void AddConsumerRepository(this IServiceCollection services)
        {
            services.AddSingleton<IConsumerHubRepository, ConsumerHubRepository>();
        }

        public static void AddConsumerHubService(this IServiceCollection services)
        {
            services.AddSingleton<IConsumerHubService, ConsumerHubService>();
        }
    }
}
