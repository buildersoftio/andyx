using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Buildersoft.Andy.X.Core.Contexts.Subscriptions
{
    public class SubscriptionPositionContext : DbContext
    {
        private readonly string _positionLogDbLocationFile;

        public SubscriptionPositionContext(string tenant, string product, string component, string topic, string subscription)
        {
            _positionLogDbLocationFile = TenantLocations.GetSubscriptionPositionLogFile(tenant, product, component, topic, subscription);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_positionLogDbLocationFile}");
        }

        public DbSet<SubscriptionPosition> CurrentPosition { get; set; }
    }
}
