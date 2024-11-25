using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherDataKevDan.Models;


public class WDContext : DbContext

{
    private const string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Weather;Integrated Security=True;";
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }


    public DbSet<WeatherData> WeatherData { get; set; }
}



//DataImporter.ImportCsvToDatabase("C:\\Users\\dnejd\\OneDrive\\Skrivbord\\VäderData.csv"); ////Importerar data till tabellen WeatherData. Välj din sökväg.







