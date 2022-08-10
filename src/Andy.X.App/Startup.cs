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
                    Title = "Buildersoft Andy X",
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

            services.AddAuthentication("UserAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("UserAuthentication", null);

            services.AddSerilogLoggingConfiguration(Configuration);
            services.AddSingleton<ApplicationService>();

            services.AddClusterRepository();
            services.AddClusterHubService();
            services.AddClusterService();

            services.AddAppFactoryMethods();
            services.AddProducerFactoryMethods();
            services.AddClusterFactoryMethods();
            services.AddProducerSubscriptionFactoryMethods();


            services.AddOrchestratorService();
            services.AddInboundMessageServices();
            services.AddOutboundMessageServices();

            services.AddTenantMemoryRepository();

            services.AddConsumerRepository();
            services.AddProducerRepository();

            services.AddSubscriptionHubService();
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
                    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v3/swagger.json", "Andy X v3"));
                }
            }

            app.UseApplicationService(serviceProvider);
            app.UseTenantMemoryRepository(serviceProvider);

            // This part is not needed. keep it for some time when we will add support for haproxy.
            //app.Use(async (context, next) =>
            //{
            //    var host = context.Request.Headers["Host"];
            //    var userAgent = context.Request.Headers["User-Agent"];
            //    var realIP = context.Request.Headers["X-Real-IP"];
            //    var forwardeds = context.Request.Headers["X-Forwarded-For"];
            //    var connectedInfo = new Dictionary<string, string>()
            //            {
            //                {"Host", host},
            //                {"UserAgent", userAgent},
            //                {"Real-IP", realIP},
            //                {"Forward-For", forwardeds},
            //            };
            //    await next.Invoke();
            //});


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


                // Mapping SignalR Hubs
                endpoints.MapHub<ProducerHub>("/realtime/v3/producer", opt =>
                {
                    opt.ApplicationMaxBufferSize = transportConfiguration.ApplicationMaxBufferSizeInBytes;
                    opt.TransportMaxBufferSize = transportConfiguration.TransportMaxBufferSizeInBytes;
                });
                endpoints.MapHub<ConsumerHub>("/realtime/v3/consumer", opt =>
                {
                    opt.ApplicationMaxBufferSize = transportConfiguration.ApplicationMaxBufferSizeInBytes;
                    opt.TransportMaxBufferSize = transportConfiguration.TransportMaxBufferSizeInBytes;
                });
            });
        }
    }
}
