using FeedR.Aggregator.Events;
using FeedR.Aggregator.Models;
using FeedR.Shared.Messaging;

namespace FeedR.Aggregator.Services;

internal sealed class PricingHandler : IPricingHandler
{
    private int _counter;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<PricingHandler> _logger;

    public PricingHandler(IMessagePublisher messagePublisher, ILogger<PricingHandler> logger)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
    }
    
    public async Task HandleAsync(CurrencyPair currencyPair)
    {
        if (ShouldPlaceOrder())
        {
            var orderId = Guid.NewGuid().ToString("N");
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _logger.LogInformation("Order with ID {OrderId} has been placed for symbols {Symbol}", orderId,
                currencyPair.Symbol);
            var integrationEvent = new OrderPlacedEvent(orderId, currencyPair.Symbol, timestamp);
            await _messagePublisher.PublishAsync("orders", integrationEvent);
        }
    }

    private bool ShouldPlaceOrder() => Interlocked.Increment(ref _counter) % 10 == 0;
}