using Buildersoft.Andy.X.Core.Contexts.Ledgers;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Entities.Ledgers;
using System.Linq;

namespace Buildersoft.Andy.X.Core.App.Repositories.Memory
{
    public class LedgerRepository
    {
        private readonly LedgerContext _ledgerContext;

        public LedgerRepository(string tenant, string product, string component, string topic)
        {
            _ledgerContext = new LedgerContext(TenantLocations.GetTopicLedgerLogFile(tenant, product, component, topic));

            _ledgerContext.ChangeTracker.AutoDetectChangesEnabled = false;
            _ledgerContext.Database.EnsureCreated();

        }

        public Ledger GetLastestLedgerData()
        {
            return _ledgerContext.Ledgers.OrderBy(x => x.Id).Last();
        }
    }
}
