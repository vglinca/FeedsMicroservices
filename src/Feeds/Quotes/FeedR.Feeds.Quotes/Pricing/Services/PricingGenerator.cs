using FeedR.Feeds.Quotes.Pricing.Models;

namespace FeedR.Feeds.Quotes.Pricing.Services;

internal sealed class PricingGenerator : IPricingGenerator
{
    private readonly ILogger<PricingGenerator> _logger;
    private readonly Random _random = new();
    private bool _isRunning;

    private readonly Dictionary<string, decimal> _currencyPairs = new()
    {
        ["EUR-USD"] = 1.13M,
        ["EUR-MDL"] = 20.17M,
        ["USD-MDL"] = 19.15M,
        ["RON-MDL"] = 4.08M,
        ["EUR-PLN"] = 4.64M,
        ["EUR-GBR"] = 0.85M,
        ["USD-GBR"] = 0.8M
    };

    public PricingGenerator(ILogger<PricingGenerator> logger)
    {
        _logger = logger;
    }

    public IEnumerable<string> GetSymbols() => _currencyPairs.Keys;

    public async IAsyncEnumerable<CurrencyPair> StartAsync()
    {
        _isRunning = true;
        while (_isRunning)
        {
            foreach (var (symbol, pricing) in _currencyPairs)
            {
                if (!_isRunning)
                {
                    yield break;
                }

                var nextTick = NextTick();
                var newPricing = pricing + nextTick;
                _currencyPairs[symbol] = newPricing;
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                
                _logger.LogInformation("Updated pricing for {Symbol}. {OriginalPricing:F} -> {NewPricing:F} [{Tick:F}]",
                    symbol, pricing, newPricing, nextTick);
                
                var currencyPair = new CurrencyPair(symbol, newPricing, timestamp);
                
                PricingUpdated?.Invoke(this, currencyPair);

                yield return currencyPair;

                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }

    public Task StopAsync()
    {
        _isRunning = false;
        return Task.CompletedTask;
    }

    private decimal NextTick()
    {
        var sign = _random.Next(0, 2) == 0 ? -1 : 1;
        var tick = _random.NextDouble() / 20;
        return (decimal) (sign * tick);
    }
    
    public event EventHandler<CurrencyPair>? PricingUpdated;
}