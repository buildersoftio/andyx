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
    public static class Tenants
    {
        public static void AnalyseTenant(string[] args)
        {
            List<TenantConfiguration> tenants = File.ReadAllText(ConfigurationLocations.GetTenantsConfigurationFile()).JsonToObject<List<TenantConfiguration>>();
            switch (args[1])
            {
                case "-list":
                case "-ls":
                    var table = new ConsoleTable("ID", "NAME", "PRODUCTS", "STATUS");
                    int k = 1;
                    tenants.ForEach(t =>
                    {
                        table.AddRow(k.ToString(), $"{t.Name}", t.Products.Count, "ACTIVE");
                        k++;
                    });
                    table.Write();
                    break;

                case "-get":
                    GetTenantDetails(args, tenants);
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

        private static void GetTenantDetails(string[] args, List<TenantConfiguration> tenants)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Wrong input.");
                return;
            }
            var name = args[2].Replace("name=", "");
            var tenant = tenants.Where(tenant => tenant.Name == name).FirstOrDefault();
            if (tenant != null)
            {
                var indexOfTenant = tenants.IndexOf(tenant) + 1;
                var table = new ConsoleTable("ID", "NAME", "ALLOW PRODUCT CREATION", "ENABLE ENCRYPTION", "DIGITAL SIGNATURE", "CERTIFICATE PATH", "GEO-REPLICATION", "LOGGING");
                table.AddRow(
                    indexOfTenant,
                    tenant.Name,
                    tenant.Settings.AllowProductCreation,
                    tenant.Settings.EnableEncryption,
                    tenant.Settings.DigitalSignature,
                    tenant.Settings.CertificatePath,
                    tenant.Settings.EnableGeoReplication,
                    tenant.Settings.Logging.ToString());

                table.Write();
            }
            else
            {
                Console.WriteLine("Invalid tenant name.");
            }
        }
    }
}
