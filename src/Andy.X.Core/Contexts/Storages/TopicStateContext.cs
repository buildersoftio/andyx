using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Microsoft.EntityFrameworkCore;

namespace Buildersoft.Andy.X.Core.Contexts.Storages
{
    public class TopicStateContext : DbContext
    {
        private readonly string _topicStatusLocation;

        public TopicStateContext(string topicStatusLocation)
        {
            _topicStatusLocation = topicStatusLocation;
        }

        public TopicStateContext(string tenant, string product, string component, string topic)
        {
            _topicStatusLocation = TenantLocations.GetTopicStateFile(tenant, product, component, topic);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_topicStatusLocation}");
        }

        public DbSet<TopicState> TopicStates { get; set; }
    }
}
