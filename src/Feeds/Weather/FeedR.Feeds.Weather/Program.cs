using FeedR.Feeds.Weather.Services;
using FeedR.Shared.HTTP;
using FeedR.Shared.Redis;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Streaming;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient()
    .AddRedis(builder.Configuration)
    .AddSerialization()
    .AddStreaming(x => x.UseRedisStreaming())
    .AddHttpApiClient<IWeatherFeedService, WeatherFeedService>()
    .AddHostedService<WeatherBackgroundService>();

var app = builder.Build();

app.MapGet("/", () => "FeedR Weather feed");

app.Run();