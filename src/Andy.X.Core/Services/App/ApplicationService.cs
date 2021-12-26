using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Buildersoft.Andy.X.Core.Services.App
{
    public class ApplicationService
    {
        public ApplicationService(ILogger<ApplicationService> logger)
        {
            var generalColor = Console.ForegroundColor;

            Console.WriteLine("                   Starting Buildersoft Andy X");
            Console.WriteLine("                   Copyright (C) 2021 Buildersoft LLC");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  ###"); Console.ForegroundColor = generalColor; Console.WriteLine("      ###");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("    ###"); Console.ForegroundColor = generalColor; Console.Write("  ###");
            Console.WriteLine("       Andy X 2.0.0-rc3. Copyright (C) 2021 Buildersoft LLC");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("      ####         "); Console.ForegroundColor = generalColor; Console.WriteLine("Licensed under the Apache License 2.0.  See https://bit.ly/3DqVQbx");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("    ###  ###");
            Console.Write("  ###      ###     "); Console.ForegroundColor = generalColor; Console.WriteLine("Andy X is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integrations");
            Console.WriteLine("");


            Console.WriteLine("                   Starting Buildersoft Andy X Node...");
            Console.WriteLine("\n");
            logger.LogInformation("Andy X Node is ready");
        }
    }
}
