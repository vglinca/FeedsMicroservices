using FeedR.Feeds.Quotes.Pricing.Requests;
using FeedR.Feeds.Quotes.Pricing.Services;
using FeedR.Shared.Redis;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Streaming;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSerialization()
    .AddStreaming()
    .AddRedis(builder.Configuration)
    .AddRedisStreaming()
    .AddHostedService<PricingBackgroundService>()
    .AddSingleton<PricingRequestChannel>()
    .AddSingleton<IPricingGenerator, PricingGenerator>();

var app = builder.Build();

app.MapGet("/", () => "FeedR Quotes feed");

app.MapPost("pricing/start", async (PricingRequestChannel channel) =>
{
    await channel.Requests.Writer.WriteAsync(new StartPricingRequest());
    return Results.Ok();
});

app.MapPost("pricing/stop", async (PricingRequestChannel channel) =>
{
    await channel.Requests.Writer.WriteAsync(new StopPricingRequest());
    return Results.Ok();
});

app.Run();