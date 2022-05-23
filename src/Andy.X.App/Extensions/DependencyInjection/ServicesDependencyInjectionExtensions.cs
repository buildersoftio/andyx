using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api.Lineage;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Services.Api;
using Buildersoft.Andy.X.Core.Services.Api.Lineage;
using Buildersoft.Andy.X.Core.Services.Inbound;
using Buildersoft.Andy.X.Router.Services.Orchestrators;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class ServicesDependencyInjectionExtensions
    {
        public static void AddRestServices(this IServiceCollection services)
        {
            services.AddSingleton<ITenantService, TenantService>();
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IComponentService, ComponentService>();
            services.AddSingleton<ITopicService, TopicService>();
            services.AddSingleton<IStreamLineageService, StreamLineageService>();
        }

        public static void AddInboundMessageServices(this IServiceCollection services)
        {
            services.AddSingleton<IInboundMessageService, InboundMessageService>();
        }

        public static void AddOrchestratorService(this IServiceCollection services)
        {
            services.AddSingleton<IOrchestratorService, OrchestratorService>();
        }
    }
}
