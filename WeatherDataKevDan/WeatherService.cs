﻿using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
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
            .OrderByDescending(d => d.AverageHumidity) // Sortera från lägsta till högsta
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
    public List<(DateTime Date, double SeasonTemp, string Season)> GetSeason(string place)
    {
        var groupedData = _context.WeatherData
            .Where(w => w.Temp.HasValue && w.Plats == "Ute")
            .GroupBy(w => w.Datum.Date)
            .Select(g => new
            {
                Date = g.Key,
                SeasonalTemperature = g.Average(w => w.Temp.Value)
            })
            .OrderBy(d => d.Date)
            .ToList();
        List<(DateTime Date, double SeasonTemp, string Season)> result = new List<(DateTime, double, string)>();
        int consecutiveDaysBelowZero = 0;
        bool winterStarted = false;
        //DateTime winterStartDate = default;
        foreach (var g in groupedData)
        {
            string seasonCalc;
            if (g.SeasonalTemperature <= 0.0)
            {
                consecutiveDaysBelowZero++;
                if (consecutiveDaysBelowZero == 5)
                {
                    winterStarted = true; //Vintern har kommit
                }
                seasonCalc = winterStarted ? "Vinter" : "Höst"; //Om vinter har startat eller är på väg att starta
            }
            else
            {
                if (winterStarted)
                {
                    seasonCalc = "Vinter"; //Håller vintern som aktiv så länge
                }
                else
                {
                    seasonCalc = "Höst"; //Annars är det höst
                }
                consecutiveDaysBelowZero = 0; //Återställ räknaren om temperaturen är över 0 grader
            }
            result.Add((g.Date, g.SeasonalTemperature, seasonCalc));
        }
        return result;
    }
}

