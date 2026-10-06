using br.com.fiap.cloudgames.Catalog.Application.DTOs;

namespace br.com.fiap.cloudgames.Catalog.Application.Repositories;

public interface IGameSearchRepository
{
    Task<IEnumerable<GameIndexDocument>> SearchAsync(string query);
}