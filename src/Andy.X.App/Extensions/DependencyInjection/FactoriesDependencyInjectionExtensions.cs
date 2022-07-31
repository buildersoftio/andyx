using Buildersoft.Andy.X.Core.Abstractions.Factories.Clusters;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Factories.Clusters;
using Buildersoft.Andy.X.Core.Factories.Consumers;
using Buildersoft.Andy.X.Core.Factories.Producers;
using Buildersoft.Andy.X.Core.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Factories.Tenants;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class FactoriesDependencyInjectionExtensions
    {


        public static void AddAppFactoryMethods(this IServiceCollection services)
        {
            services.AddSingleton<ITenantFactory, TenantFactory>();
        }

        public static void AddProducerFactoryMethods(this IServiceCollection services)
        {
            services.AddSingleton<IProducerFactory, ProducerFactory>();
        }

        public static void AddClusterFactoryMethods(this IServiceCollection services)
        {
            services.AddSingleton<IClusterFactory, ClusterFactory>();
        }

        public static void AddProducerSubscriptionFactoryMethods(this IServiceCollection services)
        {
            services.AddSingleton<IConsumerFactory, ConsumerFactory>();
            services.AddSingleton<ISubscriptionFactory, SubscriptionFactory>();
        }
    }
}
