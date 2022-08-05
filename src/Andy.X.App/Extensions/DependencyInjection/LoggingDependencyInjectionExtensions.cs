using Buildersoft.Andy.X.IO.Locations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class LoggingDependencyInjectionExtensions
    {
        public static void AddSerilogLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            string logFileLocatiom = ConfigurationLocations.NodeLoggingFile();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File(logFileLocatiom, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.Console(outputTemplate:
            //        "[{Timestamp:HH:mm:ss} {Level:u15}] {Message:lj}{NewLine}{Exception}")
            //    .CreateLogger();
        }
    }
}
