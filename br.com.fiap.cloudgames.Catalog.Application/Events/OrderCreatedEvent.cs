using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.Events
{
    public class OrderCreatedEvent
    {
        public Guid EventId { get; init; }
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid UserId { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
    }
}
