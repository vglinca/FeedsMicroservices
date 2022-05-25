using FeedR.Aggregator.Services;
using FeedR.Shared.Messaging;
using FeedR.Shared.Observability;
using FeedR.Shared.Pulsar;
using FeedR.Shared.Redis;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Streaming;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSerialization()
    .AddStreaming(cfg => cfg.UseRedisStreaming())
    .AddRedis(builder.Configuration)
    .AddHostedService<PricingStreamBackgroundService>()
    .AddHostedService<WeatherStreamBackgroundService>()
    .AddSingleton<IPricingHandler, PricingHandler>()
    .AddMessaging(cfg => cfg.UsePulsar());

var app = builder.Build();
app.UseCorrelationId();
app.MapGet("/", async ctx =>
{
    await ctx.Response.WriteAsync($"FeedR Aggregator.");
});

app.Run();