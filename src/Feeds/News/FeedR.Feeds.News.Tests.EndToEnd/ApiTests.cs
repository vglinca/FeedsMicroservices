using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FeedR.Feeds.News.Messages;
using FeedR.Shared.Streaming;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace FeedR.Feeds.News.Tests.EndToEnd;

[ExcludeFromCodeCoverage]
public class ApiTests
{
    [Fact]
    public async Task get_base_endpoint_should_return_status_ok_and_service_name()
    {
        var response = await _client.GetAsync("/");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldBe("FeedR News feed");
    }

    [Fact]
    public async Task post_news_should_return_status_accepted_and_publish_event()
    {
        var tcs = new TaskCompletionSource<NewsPublishedEvent>();
        var subscriber = _app.Services.GetRequiredService<IStreamSubscriber>();
        await subscriber.SubscribeAsync<NewsPublishedEvent>("news", message =>
        {
            tcs.SetResult(message);
        });
        
        var request = new PublishNews("test news", "test category");
        var response = await _client.PostAsJsonAsync("news", request);
        
        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        var @event = await tcs.Task;
        @event.ShouldNotBeNull();
        @event.Title.ShouldBe(request.Title);
        @event.Category.ShouldBe(request.Category);
    }

    #region Arrange

    private readonly NewsTestApp _app;
    private readonly HttpClient _client;
    
    public ApiTests()
    {
        _app = new NewsTestApp();
        _client = _app.CreateClient();
    }

    #endregion
}