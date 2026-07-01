using br.com.fiap.cloudgames.Catalog.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.ValueObjects
{
    public class Price
    {
        public decimal PriceValue { get; }

        public Price(decimal priceValue)
        {
            var errors = new List<string>();

            if (priceValue < 0)
                errors.Add("Price can't be negative.");

            if (decimal.Round(priceValue, 2) != priceValue)
                errors.Add("Price cannot have more than two decimal places.");

            if (errors.Any())
                throw new DomainException(errors);

            PriceValue = priceValue;
        }
    }
}
