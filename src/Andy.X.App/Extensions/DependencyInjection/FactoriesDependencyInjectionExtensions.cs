using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Factories.Consumers;
using Buildersoft.Andy.X.Core.Factories.Producers;
using Buildersoft.Andy.X.Core.Factories.Storages;
using Buildersoft.Andy.X.Core.Factories.Tenants;
using Microsoft.Extensions.DependencyInjection;

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

        public static void AddProducerFactoryMethods(this IServiceCollection services)
        {
            services.AddSingleton<IProducerFactory, ProducerFactory>();
        }

        public static void AddConsumerFactoryMethods(this IServiceCollection services)
        {
            services.AddSingleton<IConsumerFactory, ConsumerFactory>();
        }
    }
}
