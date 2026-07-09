using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Library> _library;

        public LibraryRepository(AppDbContext context)
        {
            _context = context;
            _library = context.Libraries;
        }

        public async Task AddAsync(Library library)
        {
            await _library.AddAsync(library);
        }

        public Task UpdateAsync(Library library)
        {
            _library.Update(library);
            return Task.CompletedTask;
        }

        public async Task<Library?> GetByIdAsync(Guid id)
        {
            return await _library.Where(l => l.UserId == id).FirstOrDefaultAsync();
        }
    }
}
