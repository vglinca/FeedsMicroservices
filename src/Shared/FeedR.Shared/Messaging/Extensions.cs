using FeedR.Shared.Pulsar;
using Microsoft.Extensions.DependencyInjection;

namespace FeedR.Shared.Messaging;

public static class Extensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, 
        Action<IPulsarConfiguration>? configure = default)
    {
        services
            .AddSingleton<IMessagePublisher, DefaultMessagePublisher>()
            .AddSingleton<IMessageSubscriber, DefaultMessagSubscriber>();
        
        configure?.Invoke(new PulsarConfiguration(services));
        
        return services;
    }
}