using br.com.fiap.cloudgames.Catalog.Application.Events;
using br.com.fiap.cloudgames.Catalog.Application.Publishers;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Config;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Messaging.Publishers
{
    public class OrderCreatedEventPublisher : RabbitMqMessagePublisher, IOrderCreatedEventPublisher
    {
        private readonly IOptions<RabbitMqSettings> _options;
        public OrderCreatedEventPublisher(RabbitMqConnection connection, IOptions<RabbitMqSettings> options) : base(connection)
        {
            _options = options;
        }

        public async Task PublishAsync(OrderCreatedEvent message)
        {
            await base.PublishAsync<OrderCreatedEvent>(_options.Value.OrderCreatedEvent.Exchange, _options.Value.OrderCreatedEvent.RoutingKey, message);
        }
    }
}
