using System.Linq;
using WeatherDataKevDan.Models;

public class WeatherService
{
    private readonly WDContext _context;

    public WeatherService(WDContext context)
    {
        _context = context;
    }


    //Metod för att ge medeltemperaturen för den dagen användaren anger, inne eller ute. Menyval 1.
    public string GetAverageTemperature(DateTime selectedDate, string place)
    {
        var weatherDataForSelectedDate = _context.WeatherData
            .Where(wd => wd.Datum.Date == selectedDate.Date &&
                         wd.Plats.ToLower() == place.ToLower() &&
                         wd.Temp.HasValue)
            .ToList();

        if (weatherDataForSelectedDate.Any())
        {
            double averageTemp = weatherDataForSelectedDate.Average(wd => wd.Temp.Value);
            return averageTemp.ToString("F1");
        }
        else
        {
            return $"Ingen data finns för {place.ToLower()} på det valda datumet.";
        }
    }


    //Metod för att sortera på varmaste till kallaste dag, inne eller ute. Menyval 2.
    public List<(DateTime Date, double AverageTemperature)> GetDaysSortedByTemperature(string place)
    {
        var groupedData = _context.WeatherData
            .Where(w => w.Temp.HasValue && w.Plats.ToLower() == place.ToLower()) // Filtrera på plats och giltig temp
            .GroupBy(w => w.Datum.Date) // Gruppera per datum
            .Select(g => new
            {
                Date = g.Key,
                AverageTemperature = g.Average(w => w.Temp.Value) // Beräkna medeltemperatur
            })
            .OrderByDescending(d => d.AverageTemperature) // Sortera från högsta till lägsta
            .ToList();

        // Konvertera till en lista av tuples
        return groupedData.Select(g => (g.Date, g.AverageTemperature)).ToList();
    }

    //Metod för att sortera på torraste till fuktigaste dag, inne eller ute. Menyval 3. 
    public List<(DateTime Date, double AverageHumidity)> GetDaysSortedByHumidity(string place)
    {
        var groupedData = _context.WeatherData
            .Where(w => w.Luftfuktighet.HasValue && w.Plats.ToLower() == place.ToLower()) // Filtrera på plats och giltig luftfuktighet
            .GroupBy(w => w.Datum.Date) // Gruppera per datum
            .Select(g => new
            {
                Date = g.Key,
                AverageHumidity = g.Average(w => w.Luftfuktighet.Value) // Beräkna medelluftfuktighet
            })
            .OrderBy(d => d.AverageHumidity) // Sortera från lägsta till högsta
            .ToList();

        // Konvertera till en lista av tuples
        return groupedData.Select(g => (g.Date, g.AverageHumidity)).ToList();
    }


}

