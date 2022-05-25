using System.Reflection;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using FeedR.Shared.Messaging;
using FeedR.Shared.Serialization;
using Microsoft.Extensions.Logging;
using IMessage = FeedR.Shared.Messaging.IMessage;

namespace FeedR.Shared.Pulsar;

internal sealed class PulsarMessageSubscriber : IMessageSubscriber
{
    private readonly ISerializer _serializer;
    private readonly ILogger<PulsarMessageSubscriber> _logger;
    private readonly IPulsarClient _pulsarClient;
    private readonly string _consumerName;

    public PulsarMessageSubscriber(ISerializer serializer, ILogger<PulsarMessageSubscriber> logger)
    {
        _serializer = serializer;
        _logger = logger;
        _pulsarClient = PulsarClient.Builder().Build();
        _consumerName = Assembly.GetEntryAssembly()?.FullName?.Split(",")[0]
            .ToLowerInvariant() ?? string.Empty;
    }

    public async Task SubscribeAsync<T>(string topic, Action<MessageEnvelope<T>> handler) where T : class, IMessage
    {
        var subscription = $"{_consumerName}_{topic}";
        var consumer = _pulsarClient.NewConsumer()
            .SubscriptionName(subscription)
            .Topic($"persistent://public/default/{topic}")
            .Create();

        await foreach (var message in consumer.Messages())
        {
            var producer = message.Properties["producer"];
            var customId = message.Properties["custom_id"];
            var correlationId = message.Properties["correlation_id"];
            _logger.LogInformation(
                "Received a message with ID {MessageId} from producer: {Producer} with custom ID: {CustomId}",
                message.MessageId, producer, customId);
            var payload = _serializer.DeserializeBytes<T>(message.Data.FirstSpan);

            if (payload is not null)
            {
                var json = _serializer.Serialize(payload);
                _logger.LogInformation("Received payload: {Payload}", json);
                handler(new MessageEnvelope<T>(payload, correlationId));
            }

            await consumer.Acknowledge(message);
        }
    }
}