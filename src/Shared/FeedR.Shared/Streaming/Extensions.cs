using FeedR.Shared.Redis.Streaming;
using Microsoft.Extensions.DependencyInjection;

namespace FeedR.Shared.Streaming;

public static class Extensions
{
    public static IServiceCollection AddStreaming(this IServiceCollection services, 
        Action<IRedisStreamingConfiguration>? configure = default)
    {
        services
            .AddSingleton<IStreamPublisher, DefaultStreamPublisher>()
            .AddSingleton<IStreamSubscriber, DefaultStreamSubscriber>();
        
        configure?.Invoke(new RedisStreamingConfiguration(services));
        
        return services;
    }
}