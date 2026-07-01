using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Config
{
    public class RabbitMqSettings
    {
        public required string URI { get; set; }
        public required RabbitMqQueueDetailsSettings UserCreatedEvent { get; set; }
    }
}
