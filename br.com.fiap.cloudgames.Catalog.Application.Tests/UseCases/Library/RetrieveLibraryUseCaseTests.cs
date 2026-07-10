using br.com.fiap.cloudgames.Catalog.Application.Abstractions;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Library.RetrieveLibrary;
using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;
using Moq;

namespace br.com.fiap.cloudgames.Catalog.Application.Tests.UseCases.Library;

public class RetrieveLibraryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenLibraryExists_ShouldReturnOwnedGames()
    {
        var userId = Guid.NewGuid();
        var game = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game.CreateGame("Game A", "Description", "Story", "Franchise", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), AgeRating.LIVRE, [GameModes.SinglePlayer], new Publisher("Publisher"), [new Developer("Developer")], new Price(10));
        var library = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library.Create(userId);
        library.AddGame(new OwnedGame(game.Id, Guid.NewGuid(), DateTime.UtcNow));
        var libraries = new Mock<ILibraryRepository>(MockBehavior.Strict);
        var games = new Mock<IGameRepository>(MockBehavior.Strict);
        var currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);
        currentUser.SetupGet(x => x.UserId).Returns(userId);
        libraries.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(library);
        games.Setup(x => x.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids => ids.Single() == game.Id))).ReturnsAsync([game]);

        var response = await new RetrieveLibraryUseCase(libraries.Object, games.Object, currentUser.Object).ExecuteAsync();

        Assert.Equal(game.Title, response.OwnedGames[game.Id.ToString()]);
    }

    [Fact]
    public async Task ExecuteAsync_WhenLibraryDoesNotExist_ShouldThrow()
    {
        var libraries = new Mock<ILibraryRepository>(MockBehavior.Strict);
        var games = new Mock<IGameRepository>(MockBehavior.Strict);
        var currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);
        var userId = Guid.NewGuid();
        currentUser.SetupGet(x => x.UserId).Returns(userId);
        libraries.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library?)null);

        var exception = await Assert.ThrowsAsync<ApplicationException>(() => new RetrieveLibraryUseCase(libraries.Object, games.Object, currentUser.Object).ExecuteAsync());

        Assert.Equal("No Library found", exception.Message);
    }
}
