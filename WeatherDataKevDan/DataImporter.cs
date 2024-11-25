using WeatherDataKevDan.Models;
using System.Globalization;

namespace WeatherDataKevDan
{
    //Metod för att importerar data till tabellen WeatherData. Välj din sökväg i metodanropet.

    public static class DataImporter
    {
        public static void ImportCsvToDatabase(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var weatherDataList = new List<WeatherData>();

            foreach (var line in lines.Skip(1)) // Hoppa över rubrikerna
            {
                var columns = line.Split(';');

                var weatherData = new WeatherData
                {
                    Datum = DateTime.ParseExact(columns[0], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    Plats = columns[1],
                    Temp = double.TryParse(columns[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var temp) ? temp : (double?)null, //data som inte kan bli double blir null
                    Luftfuktighet = double.TryParse(columns[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var humidity) ? humidity : (double?)null, //data som inte kan bli double blir null
                };

                weatherDataList.Add(weatherData);
            }
            using (var context = new WDContext())
            {
                context.AddRange(weatherDataList);
                context.SaveChanges();
            }

        }


    }
}




