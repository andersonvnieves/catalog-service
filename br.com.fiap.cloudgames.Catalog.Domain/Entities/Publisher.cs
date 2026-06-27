using br.com.fiap.cloudgames.Catalog.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Entities
{
    public record Publisher
    {
        public String Name { get; }

        public Publisher(String name)
        {
            var errors = new List<string>();

            if (String.IsNullOrWhiteSpace(name))
                errors.Add("Publisher name is required.");

            if (errors.Any())
                throw new DomainException(errors);

            Name = name.Trim();
        }
    }
}
