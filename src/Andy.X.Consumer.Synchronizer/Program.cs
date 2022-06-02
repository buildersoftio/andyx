using Andy.X.Consumer.Synchronizer.Loggers;
using Andy.X.Consumer.Synchronizer.Services;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.App.Topics;
using Newtonsoft.Json;
using System;

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

            var tempDir = TenantLocations.GetTempMessageUnAckedTopicRootDirectory(tenant, product, component, topicDetails.Name);

            MessageAcknowledgementService messageAcknoledgmentService = new MessageAcknowledgementService();
            MessageAcknowledgementService msgDeleteService = new MessageAcknowledgementService();

            messageAcknoledgmentService.ReadMessages(tempDir);
            if (messageAcknoledgmentService.StoreMessages() == true)
                messageAcknoledgmentService.RemoveBinFiles();

            msgDeleteService.ReadMessages(tempDir, "del_*");
            if (msgDeleteService.DeleteMessages() == true)
                msgDeleteService.RemoveBinFiles();

            Logger.Log($"Subscription Synchronizer for {tenant}/{product}/{component}/{topicDetails} message stored");
        }
    }
}
