using Microsoft.EntityFrameworkCore;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Data.Configurations;

namespace WidgetCo.Store.Infrastructure.Data
{
    public class WidgetCoDbContext : DbContext
    {
        public WidgetCoDbContext(DbContextOptions<WidgetCoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
        }
    }
}
