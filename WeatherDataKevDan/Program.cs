using Microsoft.EntityFrameworkCore;
using WeatherDataKevDan.Models;
using System.IO;
using System.Linq;
using WeatherDataKevDan;
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

            using (var context = new WDContext(optionsBuilder.Options))
            {
                // Skapa och uppdatera databasen om den inte redan finns
                context.Database.Migrate();

                // Importera data från CSV
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "väderdata.csv"); 
                DataImporter.ImportCsvToDatabase(filePath, context); 

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
                    Console.WriteLine("5. Presentera datum för meteorologisk Höst och Vinter");
                    Console.WriteLine("0. Avsluta");

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
                            Console.Clear();
                            Console.WriteLine("Sortera varmaste till kallaste dagen baserat på medeltemperatur per dag\n");

                            // Be användaren ange plats
                            Console.Write("Ange plats (Ute eller Inne): ");
                            string selectedPlace = Console.ReadLine();

                            // Anropa metoden med användarens val
                            var sortedDaysByPlace = weatherService.GetDaysSortedByTemperature(selectedPlace);

                            if (sortedDaysByPlace.Any())
                            {
                                Console.WriteLine($"\nVarmaste till kallaste dagen för {selectedPlace}:\n");

                                foreach (var day in sortedDaysByPlace)
                                {
                                    Console.WriteLine($"Datum: {day.Date.ToShortDateString()}, Medeltemperatur: {day.AverageTemperature:F2}°C");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"\nIngen data finns för {selectedPlace}.");
                            }

                            Console.WriteLine("\nTryck på en tangent för att återgå till huvudmenyn...");
                            Console.ReadKey();
                            break;

                        case "3":
                            Console.Clear();
                            Console.WriteLine("Sortera fuktigaste dagen baserat på medelluftfuktighet per dag\n");
                            Console.WriteLine("Ange plats (Ute eller Inne):");
                            string humidityPlaceInput = Console.ReadLine();

                            var humiditySortedDays = weatherService.GetDaysSortedByHumidity(humidityPlaceInput);

                            Console.WriteLine("\nTorraste till fuktigaste dagar enligt medelluftfuktighet:");
                            foreach (var day in humiditySortedDays)
                            {
                                Console.WriteLine($"Datum: {day.Date.ToShortDateString()}, Medelluftfuktighet: {day.AverageHumidity:F2}%");
                            }

                            Console.ReadKey();
                            break;

                        case "4":
                            Console.Clear();
                            Console.WriteLine("Ange plats (Ute eller Inne):");
                            string moldPlaceInput = Console.ReadLine();
                            var moldSortedDays = weatherService.GetDaysSortedByMoldRisk(moldPlaceInput);
                            Console.WriteLine("\nMögelRisk:");
                            foreach (var day in moldSortedDays)
                            {
                                Console.WriteLine($"Datum: {day.Date.ToShortDateString()}, Medelluftfuktighet: {day.AverageHumidity:F2}%, Mögelrisk: {day.MoldRisk}");
                            }
                            Console.ReadKey();
                            break;

                        case "5":
                            Console.Clear();
                            string seasonOutput = "Ute";
                            var seasonOutputData = weatherService.GetSeason(seasonOutput);
                            Console.WriteLine("Meteoroliska datum för höst och vinter:");
                            foreach (var day in seasonOutputData)
                            {
                                Console.WriteLine($"Datum: {day.Date.ToShortDateString()}, Medeltemperatur: {day.SeasonTemp:F2}%, Säsong: {day.Season}");
                            }
                            Console.ReadKey();
                            break;

                        case "0":
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
