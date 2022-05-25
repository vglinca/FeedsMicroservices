using FeedR.Shared.Observability;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("yarp"))
    .AddTransforms(t =>
    {
        t.AddRequestTransform(r =>
        {
            var correlationId = Guid.NewGuid().ToString("N");
            r.ProxyRequest.Headers.AddCorrelationId(correlationId);
            
            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();
// app.UseCorrelationId();
app.MapGet("/", () => "FeedR Gateway");
app.MapReverseProxy();

app.Run();