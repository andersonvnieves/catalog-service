using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CreateOrder
{
    public class CreateOrderRequest
    {
        public CreateOrderRequest()
        {
            GameIds = new List<string>();
        }

        [JsonPropertyName("GameIds")]
        public required ICollection<string> GameIds { get; set; }
    }
}
