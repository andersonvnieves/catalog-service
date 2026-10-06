using System.Net;
using br.com.fiap.cloudgames.Catalog.Application.DTOs;
using br.com.fiap.cloudgames.Catalog.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.SearchGame;

public class SearchGameUseCase
{
    private readonly IGameSearchRepository _elasticsearchProvider;
    private readonly ILogger<SearchGameUseCase> _logger;
    
    public SearchGameUseCase(IGameSearchRepository elasticsearchProvider, ILogger<SearchGameUseCase> logger)
    {
        _elasticsearchProvider = elasticsearchProvider;
        _logger = logger;
    }
    
    public async Task<SearchGameResponse> ExecuteAsync(SearchGameRequest request)
    {
        string decodedQuery = WebUtility.UrlDecode(request.Query);

        var games = await _elasticsearchProvider.SearchAsync(decodedQuery);

        var responseDtos = new List<GameSearchResponseDto>();

        if (games.Any())
        {
            responseDtos = games.Select(g => new GameSearchResponseDto
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Story = g.Story,
                Franchise = g.Franchise,
                ReleaseDate = g.ReleaseDate,
                AgeRating = g.AgeRating,
                GameModes = g.GameModes,
                PublisherName = g.PublisherName,
                DeveloperNames = g.DeveloperNames,
                PriceAmount = g.PriceAmount
            }).ToList();
        }
    
        return new SearchGameResponse()
        {
            SearchResult = responseDtos
        };
    }
}