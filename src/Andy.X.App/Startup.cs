using Buildersoft.Andy.X.Core.Services.App;
using Buildersoft.Andy.X.Extensions.DependencyInjection;
using Buildersoft.Andy.X.Router.Hubs.Consumers;
using Buildersoft.Andy.X.Router.Hubs.Producers;
using Buildersoft.Andy.X.Router.Hubs.Storages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Text.Json.Serialization;

namespace Andy.X.App
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
            if (Environment.GetEnvironmentVariable("ANDYX_EXPOSE_CONFIG_ENDPOINTS").ToLower() == "true")
            {
                services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            }

            services.AddSignalR(opt =>
                {
                    opt.MaximumReceiveMessageSize = null;
                })
                .AddJsonProtocol(opts =>
                {
                    opts.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Buildersoft Andy X",
                    Version = "v2",
                    Description = "Andy X is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integration.",
                    License = new OpenApiLicense() { Name = "Licensed under the Apache License 2.0", Url = new Uri("https://bit.ly/3DqVQbx") }
                });
            });

            services.AddSerilogLoggingConfiguration(Configuration);
            services.AddSingleton<ApplicationService>();
            services.AddStorageFactoryMethods();
            services.AddAppFactoryMethods();
            services.AddProducerFactoryMethods();
            services.AddConsumerFactoryMethods();

            services.AddConfigurations(Configuration);

            services.AddTenantMemoryRepository();

            services.AddStorageRepository();
            services.AddConsumerRepository();
            services.AddProducerRepository();

            services.AddStorageHubService();
            services.AddConsumerHubService();
            services.AddProducerHubService();

            services.AddRestServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                if (Environment.GetEnvironmentVariable("ANDYX_EXPOSE_CONFIG_ENDPOINTS").ToLower() == "true")
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Andy X v2"));
                }
            }

            app.UseApplicationService(serviceProvider);
            app.UseTenantMemoryRepository(serviceProvider);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                // Mapping Rest endpoints
                if (Environment.GetEnvironmentVariable("ANDYX_EXPOSE_CONFIG_ENDPOINTS").ToLower() == "true")
                    endpoints.MapControllers();

                // Mapping SignalR Hubs
                endpoints.MapHub<StorageHub>("/realtime/v2/storage");
                endpoints.MapHub<ProducerHub>("/realtime/v2/producer");
                endpoints.MapHub<ConsumerHub>("/realtime/v2/consumer");
            });
        }
    }
}
