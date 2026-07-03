using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CreateOrder
{
    public class CreateOrderResponse
    {
        public string OrderId { get; set; }
        public Dictionary<string, decimal> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
