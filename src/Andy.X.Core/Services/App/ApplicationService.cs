using Buildersoft.Andy.X.Core.Abstractions.Services.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Buildersoft.Andy.X.Core.Services.App
{
    public class ApplicationService
    {

        public ApplicationService(ILogger<ApplicationService> logger, NodeConfiguration nodeConfiguration, IServiceProvider serviceProvider)
        {
            var generalColor = Console.ForegroundColor;

            Console.WriteLine("                   Starting Buildersoft Andy X");
            //Console.WriteLine("                   Copyright (C) 2022 Buildersoft LLC");
            Console.WriteLine("                   Set your information in motion.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  ###"); Console.ForegroundColor = generalColor; Console.WriteLine("      ###");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("    ###"); Console.ForegroundColor = generalColor; Console.Write("  ###");
            //Console.WriteLine("       Andy X 3.0.0-alpha. Copyright (C) 2022 Buildersoft Inc.");
            Console.WriteLine("       Andy X 3.0.0-beta218. Developed with (love) by Buildersoft Inc.");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("      ####         "); Console.ForegroundColor = generalColor; Console.WriteLine("Licensed under the Apache License 2.0. See https://bit.ly/3DqVQbx");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("    ###  ###");
            Console.Write("  ###      ###     "); Console.ForegroundColor = generalColor; Console.WriteLine("Andy X is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integrations.");
            Console.WriteLine("");

            ExposePorts();

            Console.WriteLine("");
            Console.WriteLine("                   Starting Buildersoft Andy X...");
            Console.WriteLine("\n");
            logger.LogInformation("Starting Buildersoft Andy X...");
            logger.LogInformation("Update settings");
            logger.LogInformation($"Node_Id is '{nodeConfiguration.NodeId}'");

            if (Environment.GetEnvironmentVariable("ANDYX_EXPOSE_CONFIG_ENDPOINTS").ToLower() == "true")
                logger.LogInformation("Configuration endpoints are exposed");

            var clusterService = serviceProvider.GetService<IClusterService>();

            logger.LogInformation("Andy X is ready");


        }

        private static void ExposePorts()
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
