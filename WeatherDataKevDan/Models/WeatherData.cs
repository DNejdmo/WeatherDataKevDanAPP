namespace WeatherDataKevDan.Models
{ 
public class WeatherData
{
    public int Id { get; set; }
    public DateTime Datum { get; set; }
    public string Plats { get; set; } // "Ute" eller "Inne"
    public double? Temp { get; set; } // Tillåt null för felaktig data
    public double? Luftfuktighet { get; set; }
}

}








