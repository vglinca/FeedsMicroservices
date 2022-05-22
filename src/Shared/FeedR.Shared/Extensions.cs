using Microsoft.Extensions.Configuration;

namespace FeedR.Shared;

public static class Extensions
{
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string section)
        where TOptions : class, new()
    {
        var options = new TOptions();
        configuration.GetRequiredSection(section).Bind(options);

        return options;
    }
}