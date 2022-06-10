using Andy.X.Subscription.Synchronizer.Loggers;
using Andy.X.Subscription.Synchronizer.Services;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Utility.Synchronizers;
using Newtonsoft.Json;
using System;

namespace Andy.X.Subscription.Synchronizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // initialize Log Sink
            string tenant = args[0];
            string product = args[1];
            string component = args[2];
            Topic topicDetails = JsonConvert.DeserializeObject<Topic>(args[3]);

            var logSink = new LoggerSink(ConfigurationLocations.SubscriptionSyncLoggingFile(tenant, product, component, topicDetails.Name, DateTime.Now));

            Logger.Log($"Service for {tenant}/{product}/{component}/{topicDetails.Name} is working");

            var tempDir = TenantLocations.GetTempMessageUnAckedTopicRootDirectory(tenant, product, component, topicDetails.Name);

            MessageAcknowledgementService messageAcknoledgmentService = new MessageAcknowledgementService();
            MessageAcknowledgementService msgDeleteService = new MessageAcknowledgementService();

            // synchronise the message count for del and store.
            long readfiles = msgDeleteService.ReadMessages(tempDir, "del_*");
            if (msgDeleteService.DeleteMessages() == true)
                msgDeleteService.RemoveBinFiles();

            long updateFiles = messageAcknoledgmentService.ReadMessages(tempDir, sizeFromDeletedFiles: readfiles);
            if (messageAcknoledgmentService.StoreMessages() == true)
                messageAcknoledgmentService.RemoveBinFiles();

            Logger.Log($"{tenant}/{product}/{component}/{topicDetails.Name} count={readfiles} messages deleted");
            Logger.Log($"{tenant}/{product}/{component}/{topicDetails.Name} count={updateFiles} messages unacknowledged");
        }
    }
}
