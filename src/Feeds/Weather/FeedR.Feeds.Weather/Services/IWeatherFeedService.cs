using FeedR.Feeds.Weather.Models;

namespace FeedR.Feeds.Weather.Services;

internal interface IWeatherFeedService
{
    IAsyncEnumerable<WeatherData> StreamWeatherAsync(string location, CancellationToken cancellationToken = default);
}