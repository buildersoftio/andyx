using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Extensions.DI
{
    public static class Swagger
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Buildersoft Andy X",
                    Description = "Welcome to Buildersoft Andy X, the first distributed message streaming platform designed for .NET"
                });
            });
            return services;
        }
        public static IApplicationBuilder UseSwaggerView(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BuilderSoft Andy X");
                c.RoutePrefix = string.Empty;
            });
            return app;
        }
    }
}
