using br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging.Consumers;

namespace br.com.fiap.cloudgames.Catalog.WebAPI;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RabbitMqConnection _rabbitConnection;
    private readonly PaymentProcessedConsumer _paymentProcessedConsumer;
    private readonly UserCreatedConsumer _userCreatedConsumer;

    public Worker(ILogger<Worker> logger, RabbitMqConnection rabbitMqConnection, PaymentProcessedConsumer paymentProcessedConsumer, UserCreatedConsumer userCreatedConsumer)
    {
        _logger = logger;
        _rabbitConnection = rabbitMqConnection;
        _paymentProcessedConsumer = paymentProcessedConsumer;
        _userCreatedConsumer = userCreatedConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting Worker");

            await _paymentProcessedConsumer.ConsumeAsync();

            await Task.Delay(Timeout.Infinite, stoppingToken);
            _logger.LogInformation("Stopping Worker");
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex.Message);
        }
        finally
        {
            await _paymentProcessedConsumer.DisposeAsync();
        }
    }
}