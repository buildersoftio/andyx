using Buildersoft.Andy.X.Logic.Repositories.Dashboard;
using Buildersoft.Andy.X.Logic.Repositories.Interfaces.Dashboard;
using Buildersoft.Andy.X.Logic.Services;
using Buildersoft.Andy.X.Logic.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Extensions.DI
{
    public static class Dashboard
    {
        public static IServiceCollection AddDashboardServices(this IServiceCollection services)
        {
            services.AddSingleton<IDashboardSummaryMemoryRepository, DashboardSummaryMemoryRepository>();
            services.AddSingleton<IDashboardRestService, DashboardRestService>();

            return services;
        }
    }
}
