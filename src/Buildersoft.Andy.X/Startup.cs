using System;
using System.Text.Json.Serialization;
using Buildersoft.Andy.X.Extensions.DI;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Middleware;
using Buildersoft.Andy.X.Router.Hubs.Dashboard;
using Buildersoft.Andy.X.Router.Hubs.DataStorage;
using Buildersoft.Andy.X.Router.Hubs.Readers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Buildersoft.Andy.X
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSignalR()
                .AddJsonProtocol(opts =>
                {
                    opts.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Here we will add Authentication and Authorization
            services.AddApiAuthentication(Configuration);
            services.AddApiAuthorization();

            // Add cors
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>

                builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });

            // Add Health Checks
            services.AddHealthChecks();

            // Add Swagger UI for Endpoints Documentation
            services.AddSwagger();

            // Repos
            services.AddSingleton<StorageMemoryRepository>();

            // Add Singletons for Router Repositories
            services.AddRouterRepository();

            // Add Andy Tenants
            services.AddTenantRepositories();

            // Add Andy Dashboard Services
            services.AddDashboardServices();

            // Implementing SignalR
            services.AddRouterServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerView();
            }

            // Logging per Rest Calls po e ndalim njehere pasi qe kemi problem me connection me SignalR ne Docker.
            // app.UseHttpReqResLogging();

            app.UseMiddleware<AuthorizationMiddleware>();

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseDashboardServices();

            // Using Andy X Router Background Services

            app.UseEndpoints(endpoints =>
            {
                // Mapping API Endpoints
                endpoints.MapControllers();

                // Mapping health checks
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });

                // Mapping SignalR Hubs
                endpoints.MapHub<DataStorageHub>("/realtime/v1/datastorage");
                endpoints.MapHub<ReaderHub>("/realtime/v1/reader");
                endpoints.MapHub<DashboardHub>("/realtime/v1/dashboard");
            });
        }
    }
}
