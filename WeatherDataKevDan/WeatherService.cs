using WeatherDataKevDan.Models;

public class WeatherService
{
    // Metod för att beräkna medeltemperaturen för ett valt datum

    public string GetAverageTemperature(DateTime selectedDate, string place)
    {
        using (var context = new WDContext())
        {
            // Filtrera alla väderdata för det valda datumet, plats och där Temp inte är null
            var weatherDataForSelectedDate = context.WeatherData
                                            .Where(wd => wd.Datum.Date == selectedDate.Date &&
                                                         wd.Plats.ToLower() == place.ToLower() &&
                                                         wd.Temp.HasValue)
                                            .ToList();

            // Beräkna medeltemperaturen
            if (weatherDataForSelectedDate.Any())
            {
                // Räkna ut medelvärdet för temperaturerna (endast de som inte är null)
                double averageTemp = weatherDataForSelectedDate.Average(wd => wd.Temp.Value);
                return averageTemp.ToString("F1"); // Format med 1 decimal
            }
            else
            {
                // Om ingen data finns, returnera ett meddelande
                return $"Ingen data finns för {place.ToLower()} på det valda datumet.";
            }
        }
    }



}




//DataImporter.ImportCsvToDatabase("C:\\Users\\dnejd\\OneDrive\\Skrivbord\\VäderData.csv"); ////Importerar data till tabellen WeatherData. Välj din sökväg.







