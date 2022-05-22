using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FeedR.Shared.Redis;

public static class Extensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration, 
        string sectionName = "redis")
    {
        var redisOptions = configuration.GetOptions<RedisOptions>(sectionName);
        services.AddSingleton(redisOptions);
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions.ConnectionString));
            
        return services;
    }
}