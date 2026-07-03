using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Library.AddGame
{
    public class AddGameRequest
    {
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public required IReadOnlyCollection<Guid> GameIds { get; set; }
    }
}
