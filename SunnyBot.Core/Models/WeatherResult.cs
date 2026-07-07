using System.Text.Json.Serialization;

namespace SunnyBot.Models;

public class WeatherResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("sys")]
    public SysInfo Sys { get; set; }
    
    [JsonPropertyName("main")]
    public MainInfo Main { get; set; }
    
    [JsonPropertyName("weather")]
    public WeatherInfo[] Weather { get; set; }
    
    [JsonPropertyName("wind")]
    public WindInfo Wind { get; set; }
}

public class SysInfo
{
    [JsonPropertyName("country")]
    public string Country { get; set; }
}
public class MainInfo
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }
    
    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }
    
    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class WeatherInfo
{
    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class WindInfo
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}