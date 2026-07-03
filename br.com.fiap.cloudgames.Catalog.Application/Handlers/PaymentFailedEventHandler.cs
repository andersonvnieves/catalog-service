using br.com.fiap.cloudgames.Catalog.Application.Events;

namespace br.com.fiap.cloudgames.Catalog.Application.Handlers;

public class PaymentFailedEventHandler
{
    public PaymentFailedEventHandler()
    {
     
    }
    
    public async Task HandleAsync(PaymentFailedEvent paymentFailedEvent)
    {
        throw new NotImplementedException();
    }
}