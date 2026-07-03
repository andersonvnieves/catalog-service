using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Repositories
{
    public interface ILibraryRepository
    {
        Task AddAsync(Library library);
        Task UpdateAsync(Library library);
        Task<Library?> GetByIdAsync(Guid id);
    }
}
