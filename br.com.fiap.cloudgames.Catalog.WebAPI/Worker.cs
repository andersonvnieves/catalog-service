using br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging.Consumers;

namespace br.com.fiap.cloudgames.Catalog.WebAPI;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly PaymentProcessedEventConsumer _paymentProcessedEventConsumer;
    private readonly PaymentFailedEventConsumer _paymentFailedEventConsumer;

    public Worker(ILogger<Worker> logger,
        PaymentProcessedEventConsumer paymentProcessedEventConsumer, 
        PaymentFailedEventConsumer paymentFailedEventConsumer)
    {
        _logger = logger;
        _paymentProcessedEventConsumer = paymentProcessedEventConsumer;
        _paymentFailedEventConsumer = paymentFailedEventConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting Worker");

            await Task.WhenAll(
                _paymentProcessedEventConsumer.ConsumeAsync(),
                _paymentFailedEventConsumer.ConsumeAsync());
            
            await Task.Delay(Timeout.Infinite, stoppingToken);
            _logger.LogInformation("Stopping Worker");
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex.Message);
        }
        finally
        {
            await _paymentProcessedEventConsumer.DisposeAsync();
            await _paymentFailedEventConsumer.DisposeAsync();
        }
    }
}