using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Subscriptions;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using Microsoft.EntityFrameworkCore;

namespace Buildersoft.Andy.X.Core.Contexts.CoreState
{
    public class CoreStateContext : DbContext
    {
        private readonly string _tenantLogLocation;
        public CoreStateContext()
        {
            _tenantLogLocation = ConfigurationLocations.GetTenantsConfigurationLogFile();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_tenantLogLocation}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>()
            .HasIndex(p => new { p.Name }).IsUnique();
        }

        // Tenant Related Tables
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSettings> TenantSettings { get; set; }
        public DbSet<TenantToken> TenantTokens { get; set; }
        public DbSet<TenantRetention> TenantRetentions { get; set; }


        // Product Related Tables
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSettings> ProductSettings { get; set; }
        public DbSet<ProductToken> ProductTokens { get; set; }
        public DbSet<ProductRetention> ProductRetentions { get; set; }


        // Component Related Products
        public DbSet<Component> Components { get; set; }
        public DbSet<ComponentSettings> ComponentSettings { get; set; }
        public DbSet<ComponentToken> ComponentTokens { get; set; }
        public DbSet<ComponentRetention> ComponentRetentions { get; set; }


        // Topic Related Tables
        public DbSet<Topic> Topics { get; set; }


        // Subscription Related Tables
        public DbSet<Subscription> Subscriptions { get; set; }

    }
}
