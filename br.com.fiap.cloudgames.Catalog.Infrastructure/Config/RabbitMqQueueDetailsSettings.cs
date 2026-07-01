using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Config
{
    public class RabbitMqQueueDetailsSettings
    {
        public required string Exchange { get; set; }
        public required string RoutingKey { get; set; }
    }
}
