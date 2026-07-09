using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
    }
}
