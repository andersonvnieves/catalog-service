using br.com.fiap.cloudgames.Catalog.Application.Publishers;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Messaging.Publishers
{
    public abstract class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly RabbitMqConnection _connection;
        private readonly string _exchange;
        private readonly string _routingKey;

        public RabbitMqMessagePublisher(RabbitMqConnection connection, string exchange, string routingKey)
        {
            _connection = connection;
            _exchange = exchange;
            _routingKey = routingKey;
        }
        public async Task PublishAsync<T>(T message)
        {
            var conn = _connection.Connection;

            var channel = await conn.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: _exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            await channel.QueueDeclareAsync(
                queue: _routingKey,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            await channel.BasicPublishAsync(
                exchange: _exchange,
                routingKey: _routingKey,
                body: body);
        }    
    }
}
