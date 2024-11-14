using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WidgetCo.Store.Infrastructure.Data
{
    public class WidgetCoDbContextFactory : IDesignTimeDbContextFactory<WidgetCoDbContext>
    {
        public WidgetCoDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<WidgetCoDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(WidgetCoDbContext).Assembly.FullName));

            return new WidgetCoDbContext(optionsBuilder.Options);
        }
    }
}
