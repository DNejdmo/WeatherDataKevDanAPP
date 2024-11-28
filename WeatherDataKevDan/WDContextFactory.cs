using Microsoft.EntityFrameworkCore;
using WeatherDataKevDan.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;

namespace WeatherDataKevDan
{
    public class WDContextFactory : IDesignTimeDbContextFactory<WDContext>
        {
            public WDContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<WDContext>();

                // Hitta din appsettings.json eller connection string på rätt sätt
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseSqlServer(connectionString); // Eller annan databas

                return new WDContext(optionsBuilder.Options);
            }
        }
}
