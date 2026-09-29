using br.com.fiap.cloudgames.Catalog.Application.DTOs;
using br.com.fiap.cloudgames.Catalog.Application.Mappers;
using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Elasticsearch;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Relational.Repositories.Elasticsearch;

public class ElasticsearchGameRepository : IGameRepository
{
    private readonly ElasticsearchProvider _elasticsearchProvider;
    private readonly IGameRepository _gameRepository;

    public ElasticsearchGameRepository(ElasticsearchProvider elasticsearchProvider, IGameRepository gameRepository)
    {
        _elasticsearchProvider = elasticsearchProvider;
        _gameRepository = gameRepository;
    }
    
    public async Task AddAsync(Game game)
    {
        await _gameRepository.AddAsync(game);

        var document = game.ToElasticDocument();
        await _elasticsearchProvider.Client.IndexAsync(document, idx => idx
            .Index("games")
            .Id(document.Id)
        );    
    }

    public async Task<Game?> GetByIdAsync(Guid id)
    {
        return await _gameRepository.GetByIdAsync(id);
    }

    public async Task UpdateAsync(Game game)
    {
        await _gameRepository.UpdateAsync(game);

        var document = game.ToElasticDocument();
        await _elasticsearchProvider.Client.IndexAsync(document, idx => idx
            .Index("games")
            .Id(document.Id)
        );
    }

    public async Task<IEnumerable<Game>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _gameRepository.GetByIdsAsync(ids);
    }
}