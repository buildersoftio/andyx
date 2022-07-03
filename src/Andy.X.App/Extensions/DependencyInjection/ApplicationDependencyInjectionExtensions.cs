using Buildersoft.Andy.X.Core.Services.App;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class ApplicationDependencyInjectionExtensions
    {
        public static void UseApplicationService(this IApplicationBuilder builder, IServiceProvider serviceProvider)
        {
            var appService = serviceProvider.GetRequiredService<ApplicationService>();
        }
    }
}
