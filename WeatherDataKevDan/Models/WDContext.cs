using Microsoft.EntityFrameworkCore;

namespace WeatherDataKevDan.Models
{
    public class WDContext : DbContext
    {
        public WDContext(DbContextOptions<WDContext> options) : base(options)
        {
        }

        public DbSet<WeatherData> WeatherData { get; set; }
    }
}







