using Buildersoft.Andy.X.Core.Services.App;
using Buildersoft.Andy.X.Extensions.DependencyInjection;
using Buildersoft.Andy.X.Handlers;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Router.Hubs.Clusters;
using Buildersoft.Andy.X.Router.Hubs.Consumers;
using Buildersoft.Andy.X.Router.Hubs.Producers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
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
            services.AddConfigurations(Configuration);
            var transportConfiguration = JsonConvert.DeserializeObject<TransportConfiguration>(File.ReadAllText(ConfigurationLocations.GetTransportConfigurationFile()));

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
                    opt.MaximumReceiveMessageSize = transportConfiguration.MaximumReceiveMessageSizeInBytes;
                    opt.ClientTimeoutInterval = new TimeSpan(0, 0, transportConfiguration.ClientTimeoutInterval);
                    opt.HandshakeTimeout = new TimeSpan(0, 0, transportConfiguration.HandshakeTimeout);
                    opt.KeepAliveInterval = new TimeSpan(0, 0, transportConfiguration.KeepAliveInterval);

                    opt.StreamBufferCapacity = transportConfiguration.StreamBufferCapacity;
                    opt.MaximumParallelInvocationsPerClient = transportConfiguration.MaximumParallelInvocationsPerClient;
                })
                .AddJsonProtocol(opts =>
                {
                    opts.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddMessagePackProtocol();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v3", new OpenApiInfo
                {
                    Title = "Andy X",
                    Version = "v3",
                    Description = "Andy X is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integration.",
                    License = new OpenApiLicense() { Name = "Licensed under the Apache License 2.0", Url = new Uri("https://bit.ly/3DqVQbx") }
                });
                c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                         new OpenApiSecurityScheme
                           {
                               Reference = new OpenApiReference
                               {
                                   Type = ReferenceType.SecurityScheme,
                                   Id = "basic"
                               }
                           },
                           Array.Empty<string>()
                    }
                });
            });

            services.AddAuthentication("Andy.X_Authorization")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Andy.X_Authorization", null);

            // Persistency Core State
            services.AddCoreRepository();
            services.AddCoreService();

            services.AddSerilogLoggingConfiguration(Configuration);
            services.AddSingleton<ApplicationService>();

            services.AddClusterRepository();
            services.AddClusterHubService();
            services.AddClusterService();
            services.AddClusterOutboundService();

            services.AddAppFactoryMethods();
            services.AddProducerFactoryMethods();
            services.AddClusterFactoryMethods();
            services.AddProducerSubscriptionFactoryMethods();


            services.AddOrchestratorService();
            services.AddInboundMessageServices();
            services.AddOutboundMessageServices();

            services.AddTenantMemoryService();
            services.AddTenantMemoryRepository();

            services.AddConsumerRepository();
            services.AddProducerRepository();

            services.AddSubscriptionHubService();
            services.AddProducerHubService();
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
                    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v3/swagger.json", "Andy X v3"));
                }
            }

            app.UseApplicationService(serviceProvider);
            app.UseTenantMemoryRepository(serviceProvider);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            var transportConfiguration = serviceProvider.GetRequiredService<TransportConfiguration>();

            app.UseEndpoints(endpoints =>
            {
                // Mapping Rest endpoints
                if (Environment.GetEnvironmentVariable("ANDYX_EXPOSE_CONFIG_ENDPOINTS").ToLower() == "true")
                    endpoints.MapControllers();

                // mapping SignaR Cluster Hub
                endpoints.MapHub<ClusterHub>("/realtime/v3/cluster", opt =>
                {
                    opt.ApplicationMaxBufferSize = transportConfiguration.ApplicationMaxBufferSizeInBytes;
                    opt.TransportMaxBufferSize = transportConfiguration.TransportMaxBufferSizeInBytes;
                });

                // Mapping SignalR Hub for Producer
                endpoints.MapHub<ProducerHub>("/realtime/v3/producer", opt =>
                {
                    opt.ApplicationMaxBufferSize = transportConfiguration.ApplicationMaxBufferSizeInBytes;
                    opt.TransportMaxBufferSize = transportConfiguration.TransportMaxBufferSizeInBytes;
                });

                // Mapping SignalR Hub for Consumer
                endpoints.MapHub<ConsumerHub>("/realtime/v3/consumer", opt =>
                {
                    opt.ApplicationMaxBufferSize = transportConfiguration.ApplicationMaxBufferSizeInBytes;
                    opt.TransportMaxBufferSize = transportConfiguration.TransportMaxBufferSizeInBytes;
                });
            });
        }
    }
}
