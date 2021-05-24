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

            services.AddControllers();
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Andy.X.App", Version = "v1" });
            });

            services.AddSerilogLoggingConfiguration(Configuration);

            services.AddStorageFactoryMethods();
            services.AddAppFactoryMethods();
            services.AddProducerFactoryMethods();

            services.AddStorageRepository();
            services.AddHubRepository();
            services.AddTenantMemoryRepository();
            services.AddConfigurations(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Andy.X.App v1"));
            }

            app.UseTenantMemoryRepository(serviceProvider);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Mapping SignalR Hubs
                endpoints.MapHub<StorageHub>("/realtime/v2/storage");
                endpoints.MapHub<ProducerHub>("/realtime/v2/producer");
                endpoints.MapHub<ConsumerHub>("/realtime/v2/consumer");
            });
        }
    }
}
