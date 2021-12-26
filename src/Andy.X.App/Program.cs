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
            if (Environment.GetEnvironmentVariable("ANDYX_ENVIRONMENT") != "")
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ANDYX_ENVIRONMENT"));

            if (Environment.GetEnvironmentVariable("ANDYX_CERTIFICATE_DEFAULT_PASSWORD") != "")
                Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password", Environment.GetEnvironmentVariable("ANDYX_CERTIFICATE_DEFAULT_PASSWORD"));

            if (Environment.GetEnvironmentVariable("ANDYX_CERTIFICATE_DEFAULT_PATH") != "")
                Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", Environment.GetEnvironmentVariable("ANDYX_CERTIFICATE_DEFAULT_PATH"));

            if (Environment.GetEnvironmentVariable("ASPNETCORE_URLS") == "")
                Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://+:443");

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
