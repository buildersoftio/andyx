using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Buildersoft.Andy.X.Core.Services.App
{
    public class ApplicationService
    {
        public ApplicationService(ILogger<ApplicationService> logger)
        {
            var generalColor = Console.ForegroundColor;

            Console.WriteLine("                   Starting Buildersoft Andy X");
            Console.WriteLine("                   Copyright (C) 2022 Buildersoft LLC");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  ###"); Console.ForegroundColor = generalColor; Console.WriteLine("      ###");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("    ###"); Console.ForegroundColor = generalColor; Console.Write("  ###");
            Console.WriteLine("       Andy X 2.1.1. Copyright (C) 2022 Buildersoft LLC");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("      ####         "); Console.ForegroundColor = generalColor; Console.WriteLine("Licensed under the Apache License 2.0.  See https://bit.ly/3DqVQbx");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("    ###  ###");
            Console.Write("  ###      ###     "); Console.ForegroundColor = generalColor; Console.WriteLine("Andy X is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integrations.");
            Console.WriteLine("");

            ExposePorts();

            Console.WriteLine("");
            Console.WriteLine("                   Starting Buildersoft Andy X Node...");
            Console.WriteLine("\n");
            logger.LogInformation("Andy X Node is ready");
            if (Environment.GetEnvironmentVariable("ANDYX_EXPOSE_CONFIG_ENDPOINTS").ToLower() == "true")
                logger.LogInformation("Configuration endpoints are exposed");
        }

        private void ExposePorts()
        {
            var exposedUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(';');
            foreach (var url in exposedUrls)
            {
                try
                {
                    var u = new Uri(url);
                    if (u.Scheme == "https")
                        Console.WriteLine($"                   Port exposed {u.Port} SSL");
                    else
                        Console.WriteLine($"                   Port exposed {u.Port}");
                }
                catch (Exception)
                {
                    if (url.StartsWith("https://"))
                        Console.WriteLine($"                   Port exposed {url.Split(':').Last()} SSL");
                    else
                        Console.WriteLine($"                   Port exposed {url.Split(':').Last()}");
                }
            }
        }
    }
}
