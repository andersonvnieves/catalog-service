using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Exceptions;

namespace br.com.fiap.cloudgames.Catalog.Domain.Tests.Aggregates;

public class LibraryTests
{
    [Fact]
    public void Create_WithUserId_ShouldCreateEmptyLibrary()
    {
        var userId = Guid.NewGuid();

        var library = Library.Create(userId);

        Assert.Equal(userId, library.UserId);
        Assert.Empty(library.OwnedGames);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrow()
    {
        var exception = Assert.Throws<DomainException>(() => Library.Create(Guid.Empty));

        Assert.Contains("UserId is required.", exception.Errors);
    }

    [Fact]
    public void AddGame_ShouldTrackOwnership_AndRejectDuplicates()
    {
        var library = Library.Create(Guid.NewGuid());
        var gameId = Guid.NewGuid();
        library.AddGame(new OwnedGame(gameId, Guid.NewGuid(), DateTime.UtcNow));

        Assert.True(library.OwnsGame(gameId));
        Assert.True(library.OwnsGames([Guid.NewGuid(), gameId]));

        var exception = Assert.Throws<DomainException>(() =>
            library.AddGame(new OwnedGame(gameId, Guid.NewGuid(), DateTime.UtcNow)));
        Assert.Contains("User already owns this game.", exception.Errors);
    }
}
