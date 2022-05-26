using Buildersoft.Andy.X.Model.Entities.Ledgers;
using Microsoft.EntityFrameworkCore;

namespace Buildersoft.Andy.X.Core.Contexts.Ledgers
{
    public class LedgerContext : DbContext
    {
        private readonly string _ledgerLogLocation;

        public LedgerContext(string ledgerLogLocation)
        {
            _ledgerLogLocation = ledgerLogLocation;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_ledgerLogLocation}");
        }

        public DbSet<Ledger> Ledgers { get; set; }

    }
}
