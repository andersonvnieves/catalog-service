using br.com.fiap.cloudgames.Catalog.Application.DTOs;
using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;

namespace br.com.fiap.cloudgames.Catalog.Application.Mappers;

public static class GameElasticMappingExtensions
{
    public static GameIndexDocument ToElasticDocument(this Game game)
    {
        return new GameIndexDocument
        {
            Id = game.Id.ToString(),
            Title = game.Title,
            Description = game.Description,
            Story = game.Story,
            Franchise = game.Franchise,
            ReleaseDate = game.ReleaseDate,
            AgeRating = (int)game.AgeRating,
            GameModes = game.GameModes?.Select(gm => gm.ToString()).ToList() ?? new(),
            PublisherName = game.Publisher?.Name,
            DeveloperNames = game.Developers?.Select(d => d.Name).ToList() ?? new(),
            PriceAmount = game.Price?.PriceValue ?? 0
        };
    }
}