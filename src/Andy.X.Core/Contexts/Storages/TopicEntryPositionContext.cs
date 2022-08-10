using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Microsoft.EntityFrameworkCore;

namespace Buildersoft.Andy.X.Core.Contexts.Storages
{
    public class TopicEntryPositionContext : DbContext
    {
        private readonly string _topicStatusLocation;

        public TopicEntryPositionContext(string topicStatusLocation)
        {
            _topicStatusLocation = topicStatusLocation;
        }

        public TopicEntryPositionContext(string tenant, string product, string component, string topic)
        {
            _topicStatusLocation = TenantLocations.GetTopicEntryPositionFile(tenant, product, component, topic);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_topicStatusLocation}");
        }

        public DbSet<TopicEntryPosition> TopicStates { get; set; }
    }
}
