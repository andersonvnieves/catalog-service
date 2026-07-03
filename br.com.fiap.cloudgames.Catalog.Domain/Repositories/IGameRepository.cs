using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Repositories
{
    public interface IGameRepository
    {
        Task AddAsync(Game game);
        Task<Game?> GetByIdAsync(Guid id);
        void Update(Game game);
        Task<IEnumerable<Game>> GetByIdsAsync(IEnumerable<Guid> ids);       
    }
}
