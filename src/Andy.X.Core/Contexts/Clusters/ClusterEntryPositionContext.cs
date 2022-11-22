using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Microsoft.EntityFrameworkCore;

namespace Buildersoft.Andy.X.Core.Contexts.Clusters
{
    public class ClusterEntryPositionContext : DbContext
    {
        private readonly string _nodeIdLocation;
        public ClusterEntryPositionContext(string nodeId)
        {
            _nodeIdLocation = ClusterLocations.GetClusterChangeLogState(nodeId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_nodeIdLocation}");
        }

        public DbSet<TopicEntryPosition> NodeEntryStates { get; set; }
    }
}
