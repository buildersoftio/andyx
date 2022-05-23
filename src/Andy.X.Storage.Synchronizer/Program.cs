using Andy.X.Storage.Synchronizer.Loggers;
using Buildersoft.Andy.X.Model.App.Topics;
using Newtonsoft.Json;
using System.Threading;

namespace Andy.X.Storage.Synchronizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string tenant = args[0];
            string product = args[1];
            string component = args[2];
            Topic topicDetails = JsonConvert.DeserializeObject<Topic>(args[3]);

            Logger.Log($"Storage Synchronizer for {tenant}/{product}/{component}/{topicDetails.Name} is online");

            Thread.Sleep(1000);
        }
    }
}
