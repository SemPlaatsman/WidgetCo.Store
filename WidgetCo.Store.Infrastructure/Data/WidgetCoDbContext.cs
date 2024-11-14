using Microsoft.EntityFrameworkCore;
using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Infrastructure.Data
{
    public class WidgetCoDbContext : DbContext
    {
        public WidgetCoDbContext(DbContextOptions<WidgetCoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WidgetCoDbContext).Assembly);
        }
    }
}
