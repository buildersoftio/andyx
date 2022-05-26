using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.IO.Locations;

namespace Andy.X.Storage.Synchronizer.Services
{
    public class StorageService
    {
        private readonly StorageContext _storageContext;

        private readonly string _tenant;
        private readonly string _product;
        private readonly string _component;
        private readonly string _topic;

        private readonly long _ledgerId;

        public StorageService(string tenant, string product, string component, string topic, long ledgerId)
        {
            var ledgerMessageLocation = TenantLocations.GetMessageLedgerFile(tenant, product, component, topic, ledgerId);
            _storageContext = new StorageContext(ledgerMessageLocation);

            _storageContext.ChangeTracker.AutoDetectChangesEnabled = false;
            _storageContext.Database.EnsureCreated();

            _tenant = tenant;
            _product = product;
            _component = component;
            _topic = topic;
            _ledgerId = ledgerId;
        }

        public long StoreMessagesIntoLedger()
        {
            // return entryCount

            return 0;
        }

        public StorageContext GetStorageContext()
        {
            return _storageContext;
        }
    }
}
