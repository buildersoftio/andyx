using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Services.CoreState;
using Buildersoft.Andy.X.Core.Services.Inbound;
using Buildersoft.Andy.X.Core.Services.Outbound;
using Buildersoft.Andy.X.Router.Services.Orchestrators;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class ServicesDependencyInjectionExtensions
    {
        public static void AddCoreService(this IServiceCollection services)
        {
            services.AddSingleton<ICoreService, CoreService>();
        }

        public static void AddInboundMessageServices(this IServiceCollection services)
        {
            services.AddSingleton<IInboundMessageService, InboundMessageService>();
        }

        public static void AddOutboundMessageServices(this IServiceCollection services)
        {
            services.AddSingleton<IOutboundMessageService, OutboundMessageService>();
        }

        public static void AddOrchestratorService(this IServiceCollection services)
        {
            services.AddSingleton<IOrchestratorService, OrchestratorService>();
        }
    }
}
