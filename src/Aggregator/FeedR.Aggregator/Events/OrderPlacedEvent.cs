using FeedR.Shared.Messaging;

namespace FeedR.Aggregator.Events;

internal record OrderPlacedEvent(string OrderId, string Symbol, long Timestamp) : IMessage;