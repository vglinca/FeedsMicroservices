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
        _messageSubscriber.SubscribeAsync<OrderPlacedEvent>("orders", message =>
        {
            _logger.LogInformation(
                "Received an order placed event. OrderId: {OrderId} - Symbol: {Symbol} - Timestamp: {Timestamp}",
                message.OrderId, message.Symbol, message.Timestamp);
        });
        
        return Task.CompletedTask;
    }
}