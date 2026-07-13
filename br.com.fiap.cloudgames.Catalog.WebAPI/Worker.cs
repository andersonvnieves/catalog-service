using br.com.fiap.cloudgames.Catalog.Application.Consumers;

namespace br.com.fiap.cloudgames.Catalog.WebAPI;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Worker");
        using var scope = _scopeFactory.CreateScope();

        var consumer = scope.ServiceProvider
               .GetRequiredService<IPaymentProcessedEventConsumer>();

        try
        {
            await consumer.ConsumeAsync();
            await Task.Delay(Timeout.Infinite, stoppingToken);
            _logger.LogInformation("Stopping Worker");
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex.Message);
        }
        finally
        {
            await consumer.DisposeAsync();
        }
    }
}