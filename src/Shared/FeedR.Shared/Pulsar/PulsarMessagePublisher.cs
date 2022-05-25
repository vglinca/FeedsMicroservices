using System.Buffers;
using System.Collections.Concurrent;
using System.Reflection;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using FeedR.Shared.Messaging;
using FeedR.Shared.Observability;
using FeedR.Shared.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using IMessage = FeedR.Shared.Messaging.IMessage;

namespace FeedR.Shared.Pulsar;

internal sealed class PulsarMessagePublisher : IMessagePublisher
{
    private readonly ConcurrentDictionary<string, IProducer<ReadOnlySequence<byte>>> _producers = new();
    private readonly ISerializer _serializer;
    private readonly ILogger<PulsarMessagePublisher> _logger;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IPulsarClient _pulsarClient;
    private readonly string _producerName;

    public PulsarMessagePublisher(ISerializer serializer, ILogger<PulsarMessagePublisher> logger,
        IHttpContextAccessor contextAccessor)
    {
        _serializer = serializer;
        _logger = logger;
        _contextAccessor = contextAccessor;
        _pulsarClient = PulsarClient.Builder().Build();
        _producerName = Assembly.GetEntryAssembly()?.FullName?.Split(",")[0]
            .ToLowerInvariant() ?? string.Empty;
    }

    public async Task PublishAsync<T>(string topic, T message) where T : class, IMessage
    {
        var producer = _producers.GetOrAdd(topic, _pulsarClient.NewProducer()
            .ProducerName(_producerName)
            .Topic($"persistent://public/default/{topic}")
            .Create());

        var correlationId = _contextAccessor.GetCorrelationId();
        var payload = _serializer.SerializeToBytes(message);
        var metadata = new MessageMetadata
        {
            ["custom_id"] = Guid.NewGuid().ToString("N"),
            ["producer"] = _producerName,
            ["correlation_id"] = correlationId
        };
        
        var messageId = await producer.Send(metadata, payload);
        
        _logger.LogInformation("Sent a message with ID {MessageId}", messageId);
    }
}