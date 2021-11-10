using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andy.X.Cli.App
{
    public static class Help
    {
        public static void ShowHelpContent()
        {
            var generalColor = Console.ForegroundColor;
            Console.WriteLine("                   Buildersoft Andy X CLI");
            Console.WriteLine("                   Copyright (C) 2021 Buildersoft LLC");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  ###"); Console.ForegroundColor = generalColor; Console.WriteLine("      ###");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("    ###"); Console.ForegroundColor = generalColor; Console.Write("  ###");
            Console.WriteLine("       Andy X 2.0.0-rc. Copyright (C) 2021 Buildersoft LLC");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("      ####         "); Console.ForegroundColor = generalColor; Console.WriteLine("Licensed under the Apache License 2.0.  See https://bit.ly/3DqVQbx");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("    ###  ###");
            Console.Write("  ###      ###     "); Console.ForegroundColor = generalColor; Console.WriteLine("Andy X is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integrations");
            Console.WriteLine("");


            Console.WriteLine("                   Buildersoft Andy X CLI Help");
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("                   Help goes here ...");
        }

        public static void ShowVersionContent()
        {
            var generalColor = Console.ForegroundColor;
            Console.WriteLine("                   Buildersoft Andy X CLI");
            Console.WriteLine("                   Copyright (C) 2021 Buildersoft LLC");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  ###"); Console.ForegroundColor = generalColor; Console.WriteLine("      ###");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("    ###"); Console.ForegroundColor = generalColor; Console.Write("  ###");
            Console.WriteLine("       Andy X 2.0.0-rc. Copyright (C) 2021 Buildersoft LLC");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("      ####         "); Console.ForegroundColor = generalColor; Console.WriteLine("Licensed under the Apache License 2.0.  See https://bit.ly/3DqVQbx");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("    ###  ###");
            Console.Write("  ###      ###     "); Console.ForegroundColor = generalColor; Console.WriteLine("Andy X is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integrations");
            Console.WriteLine("");
        }
    }
}
