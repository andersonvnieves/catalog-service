using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.Events
{
    public class PaymentProcessedEvent
    {
        public Guid EventId { get; init; }
        public Guid OrderId { get; set; }
    }
}
