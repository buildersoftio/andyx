using Andy.X.Storage.Synchronizer.Loggers;
using Andy.X.Storage.Synchronizer.Services;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Configurations;
using Newtonsoft.Json;
using System.IO;

namespace Andy.X.Storage.Synchronizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StorageConfiguration storageConfiguration =
                JsonConvert.DeserializeObject<StorageConfiguration>(File.ReadAllText(ConfigurationLocations.GetStorageConfigurationFile()));

            string tenant = args[0];
            string product = args[1];
            string component = args[2];
            Topic topicDetails = JsonConvert.DeserializeObject<Topic>(args[3]);
            //string topicDetails = args[3];

            LedgerService ledgerService;
            StorageService storageService;
            MessageService messageService;


            Logger.Log($"{tenant}/{product}/{component}/{topicDetails.Name} is processing");

            ledgerService = new(tenant, product, component, topicDetails.Name);
            long currentLedgerId = ledgerService.GetCurrentLedgerId(out long entriesCount);

            storageService = new(tenant, product, component, topicDetails.Name, currentLedgerId);
            messageService = new(ledgerService, storageService, storageConfiguration);

            var tempDir = TenantLocations.GetTempMessageToStoreTopicRootDirectory(tenant, product, component, topicDetails.Name);
            entriesCount = messageService.ReadMessages(tempDir, entriesCount, currentLedgerId);
            if (messageService.StoreMessages())
            {
                ledgerService.CreateNextLedgerId(currentLedgerId, entriesCount);
                messageService.RemoveBinFiles();
            }

            Logger.Log($"{tenant}/{product}/{component}/{topicDetails.Name} current_ledger={currentLedgerId}; current_entry={entriesCount}");
        }
    }
}
