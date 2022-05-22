using FeedR.Shared.Serialization;
using FeedR.Shared.Streaming;
using StackExchange.Redis;

namespace FeedR.Shared.Redis.Streaming;

internal sealed class RedisStreamPublisher : IStreamPublisher
{
    private readonly ISubscriber _subscriber;
    private readonly ISerializer _serializer;

    public RedisStreamPublisher(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
    {
        _serializer = serializer;
        _subscriber = connectionMultiplexer.GetSubscriber();
    }
    public Task PublishAsync<T>(string topic, T message) where T : class
    {
        var payload = _serializer.Serialize(message);
        return _subscriber.PublishAsync(topic, payload);
    }
}