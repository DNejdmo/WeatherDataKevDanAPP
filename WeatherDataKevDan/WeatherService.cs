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
    public List<(DateTime Date, double AverageHumidity, string MoldRisk)> GetDaysSortedByMoldRisk(string place)
    {
        var groupedData = _context.WeatherData
            .Where(w => w.Luftfuktighet.HasValue && w.Plats.ToLower() == place.ToLower())
            .GroupBy(w => w.Datum.Date)
            .Select(g => new
            {
                Date = g.Key,
                AverageHumidity = g.Average(w => w.Luftfuktighet.Value)
            })
            .OrderBy(d => d.AverageHumidity)
            .ToList();
        return groupedData.Select(g =>
        {
            //Uträkning för mögel risk baserad på luftfuktighetens %
            string moldRisk;
            if (g.AverageHumidity < 60)
                moldRisk = "låg risk";
            else if (g.AverageHumidity >= 60 && g.AverageHumidity < 70)
                moldRisk = "Medel risk";
            else
                moldRisk = "Hög risk";
            return (g.Date, g.AverageHumidity, moldRisk);
        }).ToList();
    }
    public DateTime? GetAutumnSeason(string place)
    {
        var groupedData = _context.WeatherData
            .Where(w => w.Temp.HasValue && w.Plats == "Ute")
            .GroupBy(w => w.Datum.Date)
            .Select(g => new
            {
                Date = g.Key,
                AutumnTemperature = g.Average(w => w.Temp.Value)
            })
            .OrderBy(d => d.Date)
            .ToList();
        for (int i = 0; i < groupedData.Count - 4; i++)
        {
            //Uträkning för hösten, längre än 10 grader och 5 dagar i följd
            if (groupedData.Skip(i).Take(5).All(g => g.AutumnTemperature >= 0 && g.AutumnTemperature <= 10))
                return groupedData[i].Date;
        }
        return null;
    }
    public DateTime? GetWinterSeason(string place)
    {
        var groupedData = _context.WeatherData
            .Where(w => w.Temp.HasValue && w.Plats == "Ute")
            .GroupBy(w => w.Datum.Date)
            .Select(g => new
            {
                Date = g.Key,
                WinterTemperature = g.Average(w => w.Temp.Value)
            })
            .OrderBy(d => d.Date)
            .ToList();
        for (int i = 0; i < groupedData.Count - 4; i++)
        {
            //Uträkning för vinter, längre än 0.0 grader och 5 dagar i följd
            if (groupedData.Skip(i).Take(5).All(g => g.WinterTemperature >= 0 && g.WinterTemperature <= 0.0))
                return groupedData[i].Date;
        }
        return null;
    }
}

