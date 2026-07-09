using br.com.fiap.cloudgames.Catalog.Application.Consumers;

namespace br.com.fiap.cloudgames.Catalog.WebAPI;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    IServiceScopeFactory _scopeFactory;
    private readonly IPaymentProcessedEventConsumer _paymentProcessedEventConsumer;

    public Worker(ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting Worker");
            using var scope = _scopeFactory.CreateScope();
            var _paymentProcessedEventConsumer = scope.ServiceProvider
                .GetRequiredService<IPaymentProcessedEventConsumer>();

            await _paymentProcessedEventConsumer.ConsumeAsync();


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
        }
    }
}