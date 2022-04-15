using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api.Lineage;
using Buildersoft.Andy.X.Core.Services.Api;
using Buildersoft.Andy.X.Core.Services.Api.Lineage;
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
    }
}
