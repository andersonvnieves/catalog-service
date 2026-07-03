using br.com.fiap.cloudgames.Catalog.Application.Events;
using br.com.fiap.cloudgames.Catalog.Application.Publishers;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Config;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Messaging.Publishers
{
    public class OrderCreatedEventPublisher : RabbitMqMessagePublisher, IOrderCreatedEventPublisher
    {
        public OrderCreatedEventPublisher(RabbitMqConnection connection, IOptions<RabbitMqSettings> options) 
            : base(connection, options.Value.OrderCreatedEvent.Exchange, options.Value.OrderCreatedEvent.RoutingKey)
        { }

        public async Task PublishAsync(OrderCreatedEvent message)
        {
            await base.PublishAsync<OrderCreatedEvent>(message);
        }
    }
}
