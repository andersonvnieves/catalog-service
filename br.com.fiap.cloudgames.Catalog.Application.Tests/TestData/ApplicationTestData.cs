using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.CreateGame;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.UpdateGame;

namespace br.com.fiap.cloudgames.Catalog.Application.Tests.TestData;

public static class ApplicationTestData
{
    public static CreateGameRequest ValidCreateGameRequest() => new()
    {
        Title = "My Game",
        Description = "A cool game",
        Story = "Once upon a time",
        Franchise = "Franchise X",
        ReleaseDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
        AgeRating = "LIVRE",
        GameModes = ["SinglePlayer"],
        Publisher = "Publisher Inc",
        Developers = ["Dev Studio"]
    };

    public static UpdateGameRequest ValidUpdateGameRequest() => new()
    {
        Id = Guid.NewGuid().ToString(),
        Title = "My Game Updated",
        Description = "A cool game - updated",
        Story = "Once upon a time - updated",
        Franchise = "Franchise X",
        ReleaseDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
        AgeRating = "LIVRE",
        GameModes = ["SinglePlayer"],
        Publisher = "Publisher Inc",
        Developers = ["Dev Studio"]
    };    
}

