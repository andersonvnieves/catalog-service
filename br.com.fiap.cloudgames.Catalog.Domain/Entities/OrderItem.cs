using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Entities
{
    public class OrderItem
    {
        public Guid Guid { get; set; }
        public Price Price { get; set; }
    }
}
