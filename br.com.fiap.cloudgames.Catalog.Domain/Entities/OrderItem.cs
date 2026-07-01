using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Exceptions;
using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Entities
{
    public class OrderItem
    {
        public Guid GameId { get; set; }
        public Price Price { get; set; }

        public OrderItem() { } //ORM

        public OrderItem(Guid gameId, Price price) 
        {
            var errors = new List<string>();

            if (gameId == Guid.Empty)
                errors.Add("GameId is required.");

            if (price is null)
                errors.Add("Price is required.");

            if (errors.Any())
                throw new DomainException(errors);

            GameId = gameId;
            Price = price;
        }
    }
}
