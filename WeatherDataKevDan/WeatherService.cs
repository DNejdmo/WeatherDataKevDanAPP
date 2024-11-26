using System.Linq;
using WeatherDataKevDan.Models;

public class WeatherService
{
    private readonly WDContext _context;

    public WeatherService(WDContext context)
    {
        _context = context;
    }

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
}
