using Microsoft.Extensions.DependencyInjection;

namespace FeedR.Shared.Pulsar;

public interface IPulsarConfiguration
{
    IServiceCollection Services { get; }
}

internal sealed record PulsarConfiguration(IServiceCollection Services) : IPulsarConfiguration;