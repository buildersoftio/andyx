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
    public static class Components
    {
        public static void AnalyseComponent(string[] args)
        {
            List<TenantConfiguration> tenants = File.ReadAllText(ConfigurationLocations.GetTenantsConfigurationFile()).JsonToObject<List<TenantConfiguration>>();
            switch (args[1])
            {
                case "--list":
                case "-ls":
                    var table = new ConsoleTable("ID", "TENANT", "PRODUCT", "NAME", "ALLOW SCHEMA VALIDATION", "ALLOW TOPIC CREATION", "TOPICS");
                    var tenantProduct = args[2].Split(".");
                    int k = 1;
                    tenants.Where(x => x.Name == tenantProduct[0])
                        .FirstOrDefault()
                        .Products.Where(p => p.Name == tenantProduct[1]).FirstOrDefault().Components.ForEach(c =>
                      {
                          table.AddRow(k.ToString(), tenantProduct[0], tenantProduct[1], $"{c.Name}", c.Settings.AllowSchemaValidation, c.Settings.AllowTopicCreation, c.Topics.Count);
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
