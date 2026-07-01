using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
