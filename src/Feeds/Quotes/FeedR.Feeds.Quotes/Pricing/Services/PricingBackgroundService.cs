using FeedR.Feeds.Quotes.Pricing.Requests;
using FeedR.Shared.Streaming;

namespace FeedR.Feeds.Quotes.Pricing.Services;

internal sealed class PricingBackgroundService : BackgroundService
{
    private readonly IPricingGenerator _pricingGenerator;
    private readonly PricingRequestChannel _requestChannel;
    private readonly ILogger<PricingBackgroundService> _logger;
    private readonly IStreamPublisher _streamPublisher;
    private int _runningStatus;
    
    public PricingBackgroundService(IPricingGenerator pricingGenerator, PricingRequestChannel requestChannel, 
        ILogger<PricingBackgroundService> logger, IStreamPublisher streamPublisher)
    {
        _pricingGenerator = pricingGenerator;
        _requestChannel = requestChannel;
        _logger = logger;
        _streamPublisher = streamPublisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Pricing Background Service has started.");
        await foreach (var request in _requestChannel.Requests.Reader.ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation("Pricing Background Service has received the {Request}", request.GetType().Name);
            var _ = request switch
            {
                StartPricingRequest => StartGeneratorAsync(),
                StopPricingRequest => StopGeneratorAsync(),
                _ => Task.CompletedTask
            };
        }
        
        _logger.LogInformation("Pricing Background Service has stopped.");
    }

    private async Task StartGeneratorAsync()
    {
        if (Interlocked.Exchange(ref _runningStatus, 1) == 1)
        {
            _logger.LogInformation("Pricing generator is already running");
            return;
        }

        await foreach (var currencyPair in _pricingGenerator.StartAsync())
        {
            _logger.LogInformation("Publishing the currency pair...");
            await _streamPublisher.PublishAsync("pricing", currencyPair);
        }
    }
    
    private async Task StopGeneratorAsync()
    {
        if (Interlocked.Exchange(ref _runningStatus, 0) == 0)
        {
            _logger.LogInformation("Pricing generator is not running");
            return;
        }

        await _pricingGenerator.StopAsync();
    }
}