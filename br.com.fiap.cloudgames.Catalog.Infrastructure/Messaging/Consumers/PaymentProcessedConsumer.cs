using System.Text.Json;
using br.com.fiap.cloudgames.Catalog.Application.Events;
using br.com.fiap.cloudgames.Catalog.Application.Handlers;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging.Consumers;

public class PaymentProcessedConsumer
{
    private readonly PaymentProcessedEventHandler _handler;

    public PaymentProcessedConsumer(PaymentProcessedEventHandler handler)
    {
        _handler = handler;
    }

    public async Task ConsumeAsync(string message)
    {
        var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(message);

        await _handler.HandleAsync(evt);
    }
}