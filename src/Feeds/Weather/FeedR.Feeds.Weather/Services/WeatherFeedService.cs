using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using FeedR.Feeds.Weather.Models;

namespace FeedR.Feeds.Weather.Services;

internal sealed class WeatherFeedService : IWeatherFeedService
{
    private readonly ILogger<WeatherFeedService> _logger;
    private readonly HttpClient _httpClient;
    private const string ApiKey = "2518c0087a884d85be9174008222306";
    private const string ApiUrl = "https://api.weatherapi.com/v1/current.json";

    public WeatherFeedService(HttpClient httpClient, ILogger<WeatherFeedService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async IAsyncEnumerable<WeatherData> StreamWeatherAsync(string location,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var url = $"{ApiUrl}?key={ApiKey}&q={location}&aqi=no";
        while (!cancellationToken.IsCancellationRequested)
        {
            var response = await _httpClient.GetFromJsonAsync<WeatherApiResponse>(url, cancellationToken);

            if (response is null)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                continue;
            }
            
            yield return new WeatherData(
                $"{response.Location.Name}, {response.Location.Region}, {response.Location.Country}",
                response.Current.TempC, response.Current.TempF, response.Current.Humidity, response.Current.WindSpeed,
                response.Current.Condition.Text);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }

    private record WeatherApiResponse(Location Location, Current Current);

    private record Location(string Name, string Region, string Country);

    private record Current(
        [property: JsonPropertyName("temp_c")] double TempC,
        [property: JsonPropertyName("temp_f")] double TempF,
        [property: JsonPropertyName("wind_kph")]
        double WindSpeed, Condition Condition, double Humidity);

    private record Condition(string Text);
}