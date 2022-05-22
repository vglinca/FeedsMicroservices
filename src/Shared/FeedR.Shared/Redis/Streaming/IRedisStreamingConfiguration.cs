using Microsoft.Extensions.DependencyInjection;

namespace FeedR.Shared.Redis.Streaming;

public interface IRedisStreamingConfiguration
{
    IServiceCollection Services { get; }
}

internal sealed record RedisStreamingConfiguration(IServiceCollection Services) : IRedisStreamingConfiguration;