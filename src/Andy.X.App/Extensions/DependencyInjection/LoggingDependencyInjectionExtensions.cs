﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Buildersoft.Andy.X.Extensions.DependencyInjection
{
    public static class LoggingDependencyInjectionExtensions
    {
        public static void AddSerilogLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
