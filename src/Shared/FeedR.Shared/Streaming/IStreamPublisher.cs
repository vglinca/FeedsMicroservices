namespace FeedR.Shared.Streaming;

public interface IStreamPublisher
{
    Task PublishAsync<T>(string topic, T message) where T : class;
}