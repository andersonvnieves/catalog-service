using br.com.fiap.cloudgames.Catalog.Application.Consumers;
using br.com.fiap.cloudgames.Catalog.Application.Events;
using br.com.fiap.cloudgames.Catalog.Application.Handlers;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging.Consumers;

public class PaymentProcessedEventConsumer: RabbitMqMessageConsumer<PaymentProcessedEvent>, IPaymentProcessedEventConsumer
    {
        private readonly PaymentProcessedEventHandler _handler;
        private readonly IOptions<RabbitMqSettings> _options;

        public PaymentProcessedEventConsumer(ILogger<PaymentProcessedEventConsumer> logger, 
            RabbitMqConnection rabbitMqConnection, 
            PaymentProcessedEventHandler handler, 
            IOptions<RabbitMqSettings> options) 
            :base(rabbitMqConnection, logger, options.Value.PaymentProcessedEvent.Exchange, options.Value.PaymentProcessedEvent.RoutingKey)
        {
            _handler = handler;
        }

        public async Task ConsumeAsync()
        {
           await base.ConsumeAsync();
        }

        protected override async Task HandleMessageAsync(PaymentProcessedEvent message)
        {
            await _handler.HandleAsync(message);
        }
    }