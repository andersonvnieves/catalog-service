using br.com.fiap.cloudgames.Catalog.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Entities
{
    public class OwnedGame
    {
        public Guid GameId { get; }
        public Guid OrderId { get; }
        public DateTime PurchaseDate { get; }

        public OwnedGame(Guid gameId, Guid orderId, DateTime purchaseDate)
        {
            var errors = new List<string>();

            if (gameId == Guid.Empty)
                errors.Add("GameId is required.");

            if (orderId == Guid.Empty)
                errors.Add("OrderId is required.");

            if (purchaseDate > DateTime.UtcNow)
                errors.Add("PurchaseDate cannot be in the future.");

            if (errors.Any())
                throw new DomainException(errors);

            GameId = gameId;
            OrderId = orderId;
            PurchaseDate = purchaseDate;
        }

    }
}
