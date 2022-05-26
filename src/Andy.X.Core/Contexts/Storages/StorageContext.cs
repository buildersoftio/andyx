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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_ledgerMessageLocation}");
        }

        public DbSet<Message> Messages { get; set; }
    }
}
