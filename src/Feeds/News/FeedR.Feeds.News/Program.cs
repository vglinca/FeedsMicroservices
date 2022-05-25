using FeedR.Feeds.News.Messages;
using FeedR.Shared.Redis;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Streaming;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddSerialization()
    .AddRedis(builder.Configuration)
    .AddStreaming(x => x.UseRedisStreaming());

var app = builder.Build();

app.MapGet("/", () => "FeedR News feed");
app.MapPost("/news", async (PublishNews news, IStreamPublisher publisher) =>
{
    // handle news
    var @event = new NewsPublishedEvent(news.Title, news.Category);
    Task.Run(() => Task.Delay(5000)).ContinueWith(t => publisher.PublishAsync("news", @event));
    // await publisher.PublishAsync("news", @event);

    return Results.Accepted();
});

app.Run();