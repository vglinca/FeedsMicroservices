using FeedR.Shared.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace FeedR.Shared.Pulsar;

public static class Extensions
{
    public static IPulsarConfiguration UsePulsar(this IPulsarConfiguration configuration)
    {
        configuration.Services
            .AddHttpContextAccessor()
            .AddSingleton<IMessagePublisher, PulsarMessagePublisher>()
            .AddSingleton<IMessageSubscriber, PulsarMessageSubscriber>();
        
        return configuration;
    }
}