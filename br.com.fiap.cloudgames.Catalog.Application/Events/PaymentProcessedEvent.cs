using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.Events
{
    public class PaymentProcessedEvent
    {
        public Guid EventId { get; init; }
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public PaymentStatus  PaymentStatus { get; set; }
        public string Name { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}
