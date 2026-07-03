using br.com.fiap.cloudgames.Catalog.Application.Consumers;
using br.com.fiap.cloudgames.Catalog.Application.Events;
using br.com.fiap.cloudgames.Catalog.Application.Handlers;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging.Consumers;

public class PaymentFailedEventConsumer: RabbitMqMessageConsumer<PaymentFailedEvent>, IPaymentFailedEventConsumer
{
    private readonly PaymentFailedEventHandler _handler;
    private readonly IOptions<RabbitMqSettings> _options;

    public PaymentFailedEventConsumer(ILogger<PaymentFailedEventConsumer> logger, 
        RabbitMqConnection rabbitMqConnection, 
        PaymentFailedEventHandler handler, 
        IOptions<RabbitMqSettings> options) 
        :base(rabbitMqConnection, logger, options.Value.PaymentFailedEvent.Exchange, options.Value.PaymentFailedEvent.RoutingKey)
    {
        _handler = handler;
    }

    public async Task ConsumeAsync()
    {
        await base.ConsumeAsync();
    }

    protected override async Task HandleMessageAsync(PaymentFailedEvent message)
    {
        await _handler.HandleAsync(message);
    }
}