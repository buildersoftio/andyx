using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Services.Subscriptions;
using Buildersoft.Andy.X.Router.Repositories.Subscriptions;
using Buildersoft.Andy.X.Router.Services.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class ConsumerDependencyInjectionExtensions
    {
        public static void AddConsumerRepository(this IServiceCollection services)
        {
            services.AddSingleton<ISubscriptionHubRepository, SubscriptionHubRepository>();
        }

        public static void AddSubscriptionHubService(this IServiceCollection services)
        {
            services.AddSingleton<ISubscriptionHubService, SubscriptionHubService>();
        }
    }
}
