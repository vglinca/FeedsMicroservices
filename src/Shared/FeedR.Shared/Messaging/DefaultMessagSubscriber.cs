namespace FeedR.Shared.Messaging;

internal sealed class DefaultMessagSubscriber : IMessageSubscriber
{
    public Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class, IMessage => Task.CompletedTask;
}