using br.com.fiap.cloudgames.Catalog.Application.DTOs;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.SearchGame;

public class SearchGameResponse
{
    public ICollection<GameSearchResponseDto> SearchResult { get; set; }
}