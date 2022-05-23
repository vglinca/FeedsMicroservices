using FeedR.Shared.Streaming;

namespace FeedR.Aggregator.Services;

internal sealed class WeatherStreamBackgroundService : BackgroundService
{
    private readonly IStreamSubscriber _streamSubscriber;
    private readonly ILogger<WeatherStreamBackgroundService> _logger;

    public WeatherStreamBackgroundService(IStreamSubscriber streamSubscriber, 
        ILogger<WeatherStreamBackgroundService> logger)
    {
        _streamSubscriber = streamSubscriber;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _streamSubscriber.SubscribeAsync<WeatherData>("weather", weather =>
        {
            _logger.LogInformation("Received weather data: {Location}: {Temperature} C, Humidity: {Humidity}, Wind: {Wind} kph, {Condition}",
                weather.Location, weather.TemperatureC, weather.Humidity, weather.WindSpeed, weather.Condition);
        });
    }
    
    internal record WeatherData(string Location, double TemperatureC, double TemperatureF, 
        double Humidity, double WindSpeed, string Condition);
}