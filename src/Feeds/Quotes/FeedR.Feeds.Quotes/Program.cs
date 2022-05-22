using FeedR.Feeds.Quotes.Pricing.Requests;
using FeedR.Feeds.Quotes.Pricing.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
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