using FeedR.Shared.Streaming;

namespace FeedR.Feeds.Weather.Services;

internal sealed class WeatherBackgroundService : BackgroundService
{
    private readonly IStreamPublisher _streamPublisher;
    private readonly ILogger<WeatherBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public WeatherBackgroundService(IServiceProvider serviceProvider, IStreamPublisher streamPublisher, 
        ILogger<WeatherBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _streamPublisher = streamPublisher;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var weatherFeedService = scope.ServiceProvider.GetRequiredService<IWeatherFeedService>();
        
        await foreach (var weather in weatherFeedService.StreamWeatherAsync("Berlin", stoppingToken))
        {
            _logger.LogInformation("{Location}: {Temperature} C, Humidity: {Humidity}, Wind: {Wind} kph, {Condition}",
                weather.Location, weather.TemperatureC, weather.Humidity, weather.WindSpeed, weather.Condition);

            await _streamPublisher.PublishAsync("weather", weather);
        }
    }
}