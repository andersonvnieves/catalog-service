using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Catalog.Domain.Aggregates
{
    public class Order
    {
        public Order() { } //ORM

        #region FactoryMethod
        public static Order Create(Guid userId, IEnumerable<OrderItem> orderItems)
        {
            Validate(userId, orderItems);
            return new Order()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                _items = new List<OrderItem>(orderItems),
                OrderStatus = OrderStatus.Pending,
                CreatedAt = DateTime.Now,
            };
        }
        #endregion

        #region Properties
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        private List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items;
        public OrderStatus OrderStatus { get; private set; }
        public DateTime CreatedAt { get; private set; }
        #endregion

        private static void Validate(Guid userId, IEnumerable<OrderItem> orderItems)
        {
            var errors = new List<string>();

            if (userId == Guid.Empty)
                errors.Add("UserId is required.");

            if (orderItems == null || !orderItems.Any())
                errors.Add("An order must contain at least one item.");

            if (orderItems.GroupBy(i => i.GameId).Any(g => g.Count() > 1))
                errors.Add("An order cannot contain duplicate games.");

            if (errors.Any())
                throw new DomainException(errors);
        }

        public void OrderHasBeenPaid()
        {
            if (OrderStatus != OrderStatus.Pending)
                throw new DomainException("Only pending orders can be paid.");

            OrderStatus = OrderStatus.Paid;
        }

        public void OrderHasBeenCancelled()
        {
            if (OrderStatus != OrderStatus.Pending)
                throw new DomainException("Only pending orders can be cancelled.");

            OrderStatus = OrderStatus.Cancelled;
        }
        
        public Price TotalAmount
        {
            get
            {
                decimal amount = 0;
                foreach (var item in _items)
                    amount += item.Price.PriceValue;
                return new Price(amount);
            }
        }
    }
}
