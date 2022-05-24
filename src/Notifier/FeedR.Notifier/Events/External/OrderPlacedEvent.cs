using FeedR.Shared.Messaging;

namespace FeedR.Notifier.Events.External;

internal record OrderPlacedEvent(string OrderId, string Symbol, long Timestamp) : IMessage;