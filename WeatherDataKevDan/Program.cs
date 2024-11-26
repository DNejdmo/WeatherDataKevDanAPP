using Microsoft.EntityFrameworkCore;
using WeatherDataKevDan.Models;
using System.IO;
using System.Linq;
using WeatherDataKevDan;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace WeatherDataKevDan
{

    internal partial class Program
    {

        private static void Main(string[] args)
        {
            // Läser appsettings.json för att hämta connection string
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Basmapp där appsettings.json ligger
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            // Skapar DbContextOptions med connection string från appsettings.json
            var optionsBuilder = new DbContextOptionsBuilder<WDContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Använder DbContextOptions för att skapa WDContext
            using (var context = new WDContext(optionsBuilder.Options))
            {
                // Skapa och uppdatera databasen om den inte redan finns
                context.Database.Migrate();
            
                var weatherService = new WeatherService(context);

                bool exit = false;

                while (!exit)
                {
                    Console.Clear(); // Rensar konsolen för en ren meny
                    Console.WriteLine("Välkommen till Kevin och Daniels väderapplikation!");
                    Console.WriteLine("\nVälj ett alternativ:");
                    Console.WriteLine("1. Beräkna medeltemperatur för ett valt datum");
                    Console.WriteLine("2. Sortera varmaste till kallaste dagen enligt medeltemperatur per dag");
                    Console.WriteLine("3. Sortera torraste till fuktigaste dagen enligt medelluftfuktighet per dag");
                    Console.WriteLine("4. Sortera minst till störst risk för mögel");
                    Console.WriteLine("5. Presentera datum för meteorologisk Höst");
                    Console.WriteLine("6. Presentera datum för meteorologisk Vinter");
                    Console.WriteLine("7. Avsluta");

                    Console.Write("Ange ditt val: ");
                    string userInput = Console.ReadLine();

                    switch (userInput)
                    {
                        case "1":

                            // Be användaren att ange ett datum
                            Console.WriteLine("Ange ett datum (i formatet YYYY-MM-DD):");
                            string dateInput = Console.ReadLine();

                            // Be användaren att ange plats (Ute eller Inne, gemener eller versaler spelar ingen roll)
                            Console.WriteLine("Ange plats (Ute eller Inne):");
                            string placeInput = Console.ReadLine();

                            // Konvertera användarens input till ett datum
                            if (DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime selectedDate))
                            {
                                // Anropa metoden för att beräkna medeltemperaturen
                                string result = weatherService.GetAverageTemperature(selectedDate, placeInput);

                                // Skriv ut resultatet
                                Console.WriteLine($"Medeltemperaturen var {result}°C. ");
                            }
                            else
                            {
                                Console.WriteLine("Felaktigt datumformat. Vänligen ange ett datum i formatet YYYY-MM-DD.");
                            }

                            Console.ReadKey();
                            break;


                        case "2":
                            //Metod för att sortera varmaste till kallaste dagen enligt medeltemperatur per dag, ute och inne. 
                            break;

                        case "3":
                            //Metod för att sortera torraste till fuktigaste dagen enligt medelluftfuktighet per dag, ute och inne
                            break;

                        case "4":
                            //Sortering av minst till störst risk för mögel, ute och inne
                            break;

                        case "5":
                            //Presentera datum för meteorologisk Höst
                            break;

                        case "6":
                            //Presentera datum för meteorologisk Vinter
                            break;

                        case "7":
                            Console.WriteLine("Avslutar programmet...");
                            exit = true;
                            break;

                        default:
                            Console.WriteLine("Ogiltigt val. Försök igen.");
                            break;
                    }

                    if (!exit)
                    {
                        Console.WriteLine("\nTryck på en tangent för att fortsätta...");
                        Console.ReadKey();
                    }
                }
            }
        }
    }
}


