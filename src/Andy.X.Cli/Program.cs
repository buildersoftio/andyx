using Andy.X.Cli.App;
using System;

namespace Andy.X.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Help.ShowHelpContent();
                return;
            }
            switch (args[0])
            {

                case "-help":
                    App.Help.ShowHelpContent();
                    break;
                // ------------------------------------------

                case "-version":
                case "-ver":
                    App.Help.ShowVersionContent();
                    break;
                // ------------------------------------------

                case "-auth":
                    Console.WriteLine("Authorization goes here.");
                    break;
                // ------------------------------------------

                case "-tenant":
                    Tenants.AnalyseTenant(args);
                    break;
                // ------------------------------------------

                case "-product":
                    Products.AnalyseProduct(args);
                    break;
                // ------------------------------------------

                case "-component":
                    Components.AnalyseComponent(args);
                    break;
                // ------------------------------------------

                case "-topic":
                    Console.WriteLine("Topic goes here.");
                    break;
                // ------------------------------------------

                case "-consumer":
                    Console.WriteLine("Consumer goes here.");
                    break;
                // ------------------------------------------

                case "-producer":
                    Console.WriteLine("Producer goes here.");
                    break;
                // ------------------------------------------

                default:
                    break;
            }
        }
    }

    // examples how andyx-cli will look like from dev point of view
    // TBD: if there are different ides

    // andyx-cli -auth -get -tenant=default -digitalsignature={string_optional}
    // andyx-cli -tenant -add -name="tenantName" -digitalSignature="string_optional"
    // andyx-cli -topic -add/-edit/-view -details="tenant/product/component/{topic_name}" -persistent/nonpersistent
    // andyx-cli -consumer -tenant=default -product=default -component=default -topic={topic_name} -name={consumerName} -type=exclusive
    // andyx-cli -producer -tenant=default -product=default -component=default -topic={topic_name} -jsoncontent="{json}"
}
