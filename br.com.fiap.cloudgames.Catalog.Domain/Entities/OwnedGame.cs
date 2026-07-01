using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Entities
{
    public class OwnedGame
    {
        #region Properties
        public Guid GameId { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public Guid OrderId { get; private set; }
        #endregion
        public OwnedGame(Guid gameId, DateTime purchaseDate, Guid orderId)
        {
            GameId = gameId;
            PurchaseDate = purchaseDate;
            OrderId = orderId;
        }

    }
}
