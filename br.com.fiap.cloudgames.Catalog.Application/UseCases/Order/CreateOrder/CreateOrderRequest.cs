using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CreateOrder
{
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public required ICollection<string> GameIds { get; set; }
    }
}
