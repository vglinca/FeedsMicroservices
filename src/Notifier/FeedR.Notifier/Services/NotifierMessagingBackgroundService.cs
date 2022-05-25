using FeedR.Notifier.Events.External;
using FeedR.Shared.Messaging;

namespace FeedR.Notifier.Services;

internal sealed class NotifierMessagingBackgroundService : BackgroundService
{
    private readonly IMessageSubscriber _messageSubscriber;
    private readonly ILogger<NotifierMessagingBackgroundService> _logger;

    public NotifierMessagingBackgroundService(IMessageSubscriber messageSubscriber,
        ILogger<NotifierMessagingBackgroundService> logger)
    {
        _messageSubscriber = messageSubscriber;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageSubscriber.SubscribeAsync<OrderPlacedEvent>("orders", messageEnvelope =>
        {
            _logger.LogInformation(
                "Received an order placed event. OrderId: {OrderId} - Symbol: {Symbol} - Timestamp: {Timestamp}. With CorrelationId: {CorrelationId}",
                messageEnvelope.Message.OrderId, messageEnvelope.Message.Symbol, messageEnvelope.Message.Timestamp,
                messageEnvelope.CorrelationId);
        });

        return Task.CompletedTask;
    }
}