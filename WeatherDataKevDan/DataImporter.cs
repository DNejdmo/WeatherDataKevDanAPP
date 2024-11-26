//klass och metod för att ladda in data från en csv-fil. Skall inte behövas göras igen.




//using WeatherDataKevDan.Models;
//using System.Globalization;

//namespace WeatherDataKevDan
//{
//    public static class DataImporter
//    {
//        public static void ImportCsvToDatabase(string filePath, WDContext context)
//        {
//            var lines = File.ReadAllLines(filePath);
//            var weatherDataList = new List<WeatherData>();

//            foreach (var line in lines.Skip(1)) // Hoppa över rubrikerna
//            {
//                var columns = line.Split(';');

//                var weatherData = new WeatherData
//                {
//                    Datum = DateTime.ParseExact(columns[0], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
//                    Plats = columns[1],
//                    Temp = double.TryParse(columns[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var temp) ? temp : (double?)null,
//                    Luftfuktighet = double.TryParse(columns[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var humidity) ? humidity : (double?)null,
//                };

//                weatherDataList.Add(weatherData);
//            }

//            context.AddRange(weatherDataList);
//            context.SaveChanges();
//        }
//    }
//}
