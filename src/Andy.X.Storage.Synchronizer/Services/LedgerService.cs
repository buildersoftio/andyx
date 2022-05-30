using Buildersoft.Andy.X.Core.Contexts.Ledgers;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Entities.Ledgers;
using System.Linq;

namespace Andy.X.Storage.Synchronizer.Services
{
    public class LedgerService
    {
        private readonly LedgerContext _ledgerContext;

        private readonly string _tenant;
        private readonly string _product;
        private readonly string _component;
        private readonly string _topic;

        public LedgerService(string tenant, string product, string component, string topic)
        {
            var ledgerLocation = TenantLocations.GetTopicLedgerLogFile(tenant, product, component, topic);
            _ledgerContext = new LedgerContext(ledgerLocation);
            // try to connect or ensure that this db log exits.

            _ledgerContext.ChangeTracker.AutoDetectChangesEnabled = false;
            _ledgerContext.Database.EnsureCreated();

            _tenant = tenant;
            _product = product;
            _component = component;
            _topic = topic;
        }


        public long GetCurrentLedgerId(out long entitiesCount)
        {
            entitiesCount = 0;
            Ledger ledger = _ledgerContext.Ledgers.Where(l => l.Status == LedgerStatus.Opened).FirstOrDefault();

            if (ledger == null)
            {
                // create the first ledger if there where no ledger log record before.
                string ledgerLocation = TenantLocations.GetMessageLedgerFile(_tenant, _product, _component, _topic, 1);
                ledger = new Ledger()
                {
                    Location = ledgerLocation,
                    CreatedDate = System.DateTimeOffset.Now,
                    Entries = 0,
                    IsDeleted = false,
                    Status = LedgerStatus.Opened,
                    Size = 0
                };

                _ledgerContext.Ledgers.Add(ledger);
                _ledgerContext.SaveChanges();


                return ledger.Id;
            }

            entitiesCount = ledger.Entries;
            return ledger.Id;
        }

        public void CreateNextLedgerId(long currentLedgerId, long entryCount)
        {
            var currentLedger = _ledgerContext.Ledgers.Find(currentLedgerId);
            currentLedger.Entries = entryCount;

            if (entryCount < 50000)
            {
                // update the entryCount to ledger_log
                _ledgerContext.Ledgers.Update(currentLedger);
                _ledgerContext.SaveChanges();
                return;
            }

            currentLedger.Status = LedgerStatus.Closed;
            _ledgerContext.Ledgers.Update(currentLedger);
            _ledgerContext.SaveChanges();

            // create new Ledger row;
            long newLedgerId = currentLedgerId + 1;
            string ledgerLocation = TenantLocations.GetMessageLedgerFile(_tenant, _product, _component, _topic, newLedgerId);

            var newLedger = new Ledger()
            {
                Location = ledgerLocation,
                CreatedDate = System.DateTimeOffset.Now,
                Entries = 0,
                IsDeleted = false,
                Status = LedgerStatus.Opened,
                Size = 0
            };
            _ledgerContext.Ledgers.Add(newLedger);
            _ledgerContext.SaveChanges();

            // Create the msg_ledger...
            var storage = new StorageService(_tenant, _product, _component, _topic, newLedgerId);
        }
    }
}
