using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Microsoft.EntityFrameworkCore;

namespace Buildersoft.Andy.X.Core.Contexts.Storages
{
    public class StorageContext : DbContext
    {
        private readonly string _ledgerMessageLocation;

        public StorageContext(string ledgerMessageLocation)
        {
            _ledgerMessageLocation = ledgerMessageLocation;
        }

        public StorageContext(string tenant, string product, string component, string topic, long ledgerId)
        {
            _ledgerMessageLocation = TenantLocations.GetMessageLedgerFile(tenant, product, component, topic, ledgerId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_ledgerMessageLocation}");
        }

        public DbSet<Message> Messages { get; set; }
    }
}
