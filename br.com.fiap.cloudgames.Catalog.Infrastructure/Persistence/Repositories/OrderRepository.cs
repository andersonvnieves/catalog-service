using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Order> _order;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
            _order = context.Orders;
        }

        public async Task AddAsync(Order order)
        {
            await _order.AddAsync(order);
        }

        public Task UpdateAsync(Order order)
        {
            _order.Update(order);
            return Task.CompletedTask;
        }

        public Task<Order?> GetByIdAsync(Guid id)
        {
            return _order.FirstOrDefaultAsync(o => o.Id == id);
        }        
    }
}
