using FeedR.Aggregator.Services;
using FeedR.Shared.Messaging;
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

app.MapGet("/", async ctx =>
{
    var requestId = ctx.Request.Headers["x-request-id"];
    await ctx.Response.WriteAsync($"FeedR Aggregator. Request Id: {requestId}");
});

app.Run();