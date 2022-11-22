using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Andy.X.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .CreateLogger();

            // SETTING environment variables for Env, Cert and default asp_net
            if (Environment.GetEnvironmentVariable("ANDYX_ENVIRONMENT") != null)
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ANDYX_ENVIRONMENT"));

            if (Environment.GetEnvironmentVariable("ANDYX_CERTIFICATE_DEFAULT_PASSWORD") != null)
                Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password", Environment.GetEnvironmentVariable("ANDYX_CERTIFICATE_DEFAULT_PASSWORD"));

            if (Environment.GetEnvironmentVariable("ANDYX_CERTIFICATE_DEFAULT_PATH") != null)
                Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", Environment.GetEnvironmentVariable("ANDYX_CERTIFICATE_DEFAULT_PATH"));

            if (Environment.GetEnvironmentVariable("ANDYX_URLS") != null)
                Environment.SetEnvironmentVariable("ASPNETCORE_URLS", Environment.GetEnvironmentVariable("ANDYX_URLS"));
            else
                Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://+:6541;http://+:6540");

            if (Environment.GetEnvironmentVariable("ANDYX_EXPOSE_CONFIG_ENDPOINTS") == null)
                Environment.SetEnvironmentVariable("ANDYX_EXPOSE_CONFIG_ENDPOINTS", "true");

            if (Environment.GetEnvironmentVariable("ANDYX_NODE_ID") == null)
                Environment.SetEnvironmentVariable("ANDYX_NODE_ID", "default_01");

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
