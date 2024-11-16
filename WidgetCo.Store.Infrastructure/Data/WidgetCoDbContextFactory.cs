using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WidgetCo.Store.Infrastructure.Data
{
    public class WidgetCoDbContextFactory : IDesignTimeDbContextFactory<WidgetCoDbContext>
    {
        public WidgetCoDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../WidgetCo.Store.Api")) // Point to API project
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<WidgetCoDbContext>();
            var connectionString = configuration.GetConnectionString("SqlServerConnectionString")
                ?? throw new InvalidOperationException("Connection string 'SqlServerConnectionString' not found.");

            optionsBuilder.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(WidgetCoDbContext).Assembly.FullName));

            return new WidgetCoDbContext(optionsBuilder.Options);
        }
    }
}
