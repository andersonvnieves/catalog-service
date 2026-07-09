using System.Text;
using System.Text.Json;
using br.com.fiap.cloudgames.Catalog.Application.Consumers;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging.Consumers;

public abstract class RabbitMqMessageConsumer<T> : IMessageConsumer, IAsyncDisposable
{
    private readonly ILogger<T> _logger;
    private readonly RabbitMqConnection _rabbitConnection;
    private IChannel? _channel;

    private readonly string _exchange;
    private readonly string _routingKey;

    protected RabbitMqMessageConsumer(
        RabbitMqConnection rabbitConnection,
        ILogger<T> logger,
        string exchange,
        string routingKey)
    {
        _rabbitConnection = rabbitConnection;
        _logger = logger;
        _exchange = exchange;
        _routingKey = routingKey;
    }

    public async Task ConsumeAsync()
    {
        _logger.LogInformation("Starting consumer");

        var connection = _rabbitConnection.Connection;
        _channel = await connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(
            exchange: _exchange,
            type: ExchangeType.Topic,
            durable: true);

        await _channel.QueueDeclareAsync(
            queue: _routingKey,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await _channel.QueueBindAsync(
            queue: _routingKey,
            exchange: _exchange,
            routingKey: _routingKey);

        var rabbitConsumer = new AsyncEventingBasicConsumer(_channel);

        rabbitConsumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<T>(message);
                if (evt is null)
                    throw new InvalidOperationException("Mensagem inválida.");

                await HandleMessageAsync(evt);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem.");
                await _channel.BasicNackAsync(
                    ea.DeliveryTag,
                    false,
                    true);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _routingKey,
            autoAck: false,
            consumer: rabbitConsumer);
    }

    protected abstract Task HandleMessageAsync(T message);

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }
    }
}