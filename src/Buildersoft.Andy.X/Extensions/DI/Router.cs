using Buildersoft.Andy.X.Data.Model.Router.Dashboard;
using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using Buildersoft.Andy.X.Data.Model.Router.Readers;
using Buildersoft.Andy.X.Router.Repositories;
using Buildersoft.Andy.X.Router.Repositories.Dashboard;
using Buildersoft.Andy.X.Router.Repositories.DataStorages;
using Buildersoft.Andy.X.Router.Repositories.Readers;
using Buildersoft.Andy.X.Router.Services.DataStorages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Buildersoft.Andy.X.Extensions.DI
{
    public static class Router
    {
        public static IServiceCollection AddRouterRepository(this IServiceCollection services)
        {
            services.AddSingleton<IHubRepository<DataStorage>, DataStorageRepository>();
            services.AddSingleton<IHubRepository<Reader>, ReaderRepository>();
            services.AddSingleton<IHubRepository<DashboardUser>, DashboardUserRepository>();

            return services;
        }

        public static IServiceCollection AddRouterServices(this IServiceCollection services)
        {
            // DataStorage Services
            services.AddSingleton<TenantService>();
            services.AddSingleton<ProductService>();
            services.AddSingleton<ComponentService>();
            services.AddSingleton<BookService>();
            services.AddSingleton<ReaderService>();
            services.AddSingleton<MessageService>();

            // Reader Services
            services.AddSingleton<X.Router.Services.Readers.ReaderService>();

            // Dashboard Services
            services.AddSingleton<X.Router.Services.Dashboard.DashboardSummaryService>();

            return services;
        }

        public static IApplicationBuilder UseDashboardServices(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<X.Router.Services.Dashboard.DashboardSummaryService>();

            return app;
        }
    }
}
