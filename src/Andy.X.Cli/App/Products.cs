using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andy.X.Cli.App
{
    public static class Products
    {
        public static void AnalyseProduct(string[] args)
        {
            List<TenantConfiguration> tenants = File.ReadAllText(ConfigurationLocations.GetTenantsConfigurationFile()).JsonToObject<List<TenantConfiguration>>();
            switch (args[1])
            {
                case "--list":
                case "-ls":
                    var table = new ConsoleTable("ID", "TENANT", "NAME", "COMPONENTS");
                    int k = 1;
                    tenants.Where(x => x.Name == args[2]).FirstOrDefault().Products.ForEach(p =>
                       {
                           table.AddRow(k.ToString(), args[2], $"{p.Name}", p.Components.Count);
                           k++;
                       });
                    table.Write();
                    break;

                case "-get":
                    //GetTenantDetails(args, tenants);
                    break;
                // ------------------------------------------

                case "-post":
                    Console.WriteLine("add new tenant");
                    break;
                // ------------------------------------------

                case "-put":
                    Console.WriteLine("update existing tenant");
                    break;
                // ------------------------------------------

                case "-delete":
                    Console.WriteLine("delete tenant");
                    break;
                // ------------------------------------------
                default:
                    break;
            }
        }
    }
}
