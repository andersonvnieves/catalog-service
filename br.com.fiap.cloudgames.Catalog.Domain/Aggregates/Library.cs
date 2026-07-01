using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Aggregates
{
    public class Library
    {
        public Guid UserId { get; set; }
        public ICollection<OwnedGame> OwnedGames { get; set; }
    }
}
