using Andy.X.Consumer.Synchronizer.Loggers;
using Andy.X.Consumer.Synchronizer.Services;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.App.Topics;
using Newtonsoft.Json;

namespace Andy.X.Consumer.Synchronizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger.Log($"Subscription Synchronizer is online");


            string tenant = args[0];
            string product = args[1];
            string component = args[2];
            Topic topicDetails = JsonConvert.DeserializeObject<Topic>(args[3]);

            MessageAcknoledgmentService messageAcknoledgmentService = new MessageAcknoledgmentService();

            var tempDir = TenantLocations.GetTempMessageUnAckedTopicRootDirectory(tenant, product, component, topicDetails.Name);
            messageAcknoledgmentService.ReadMessages(tempDir);
            if (messageAcknoledgmentService.StoreMessages() == true)
                messageAcknoledgmentService.RemoveBinFiles();

            Logger.Log($"Subscription Synchronizer for {tenant}/{product}/{component}/{topicDetails} message stored");
        }
    }
}
