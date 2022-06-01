using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Microsoft.EntityFrameworkCore;

namespace Buildersoft.Andy.X.Core.Contexts.Storages
{
    public class MessageAcknowledgementContext : DbContext
    {
        private readonly string _ackedMessageLogLocation;
        public MessageAcknowledgementContext(string tenant, string product, string component, string topic, string subscriptionName)
        {
            _ackedMessageLogLocation = TenantLocations.GetSubscriptionAcknowledgementLogFile(tenant, product, component, topic, subscriptionName);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_ackedMessageLogLocation}");
        }

        public DbSet<UnacknowledgedMessage> UnacknowledgedMessages { get; set; }
        public DbSet<AcknowledgedMessage> AcknowledgedMessages { get; set; }
        public DbSet<SkippedMessage> SkippedMessages { get; set; }
    }
}
