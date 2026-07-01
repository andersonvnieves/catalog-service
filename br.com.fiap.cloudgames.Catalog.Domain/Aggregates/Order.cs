using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Aggregates
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public ICollection<OrderItem> Items { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
