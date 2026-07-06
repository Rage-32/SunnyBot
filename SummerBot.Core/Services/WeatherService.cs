using System.Net.Http.Json;
using SummerBot.Models;

namespace SummerBot.Services;

public class WeatherService(HttpClient http, string apiKey)
{
    public async Task<WeatherResult?> GetWeatherAsync(string city)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";
        return await http.GetFromJsonAsync<WeatherResult>(url);
    }
}