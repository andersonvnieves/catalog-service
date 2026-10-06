using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.Repositories
{
    public interface IGameRepository
    {
        Task AddAsync(Game game);
        Task<Game?> GetByIdAsync(Guid id);
        Task UpdateAsync(Game game);
        Task<IEnumerable<Game>> GetByIdsAsync(IEnumerable<Guid> ids);       
    }
}
