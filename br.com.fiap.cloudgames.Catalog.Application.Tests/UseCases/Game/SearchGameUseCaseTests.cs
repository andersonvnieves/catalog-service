using System.Net;
using br.com.fiap.cloudgames.Catalog.Application.DTOs;
using br.com.fiap.cloudgames.Catalog.Application.Repositories;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.SearchGame;
using Microsoft.Extensions.Logging;
using Moq;

namespace br.com.fiap.cloudgames.Catalog.Application.Tests.UseCases.Game;

public class SearchGameUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithUrlEncodedQuery_ShouldDecodeAndReturnMappedGames()
    {
        // Arrange
        var gameSearchRepositoryMock = new Mock<IGameSearchRepository>(MockBehavior.Strict); 
        var loggerMock = new Mock<ILogger<SearchGameUseCase>>(MockBehavior.Loose);
        
        var sut = new SearchGameUseCase(gameSearchRepositoryMock.Object, loggerMock.Object);

        string rawQuery = "zelda%20breath%20of%20the%20wild";
        string decodedQuery = WebUtility.UrlDecode(rawQuery);

        var fakeIndexDocuments = new List<GameIndexDocument>
        {
            new GameIndexDocument
            {
                Id = Guid.NewGuid().ToString(),
                Title = "The Legend of Zelda",
                Description = "Open world adventure",
                Story = "A long time ago...",
                Franchise = "Zelda",
                ReleaseDate = new DateOnly(2017, 3, 3),
                AgeRating = 10,
                GameModes = new List<string> { "SinglePlayer" },
                PublisherName = "Nintendo",
                DeveloperNames = new List<string> { "Nintendo EPD" },
                PriceAmount = 299.99m
            }
        };

        gameSearchRepositoryMock
            .Setup(x => x.SearchAsync(decodedQuery))
            .ReturnsAsync(fakeIndexDocuments);

        var request = new SearchGameRequest { Query = rawQuery };

        // Act
        var response = await sut.ExecuteAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.SearchResult);
        var resultDto = Assert.Single(response.SearchResult);

        Assert.Equal(fakeIndexDocuments[0].Id, resultDto.Id);
        Assert.Equal(fakeIndexDocuments[0].Title, resultDto.Title);
        Assert.Equal(fakeIndexDocuments[0].Description, resultDto.Description);
        Assert.Equal(fakeIndexDocuments[0].Story, resultDto.Story);
        Assert.Equal(fakeIndexDocuments[0].Franchise, resultDto.Franchise);
        Assert.Equal(fakeIndexDocuments[0].ReleaseDate, resultDto.ReleaseDate);
        Assert.Equal(fakeIndexDocuments[0].AgeRating, resultDto.AgeRating);
        Assert.Equal(fakeIndexDocuments[0].GameModes, resultDto.GameModes);
        Assert.Equal(fakeIndexDocuments[0].PublisherName, resultDto.PublisherName);
        Assert.Equal(fakeIndexDocuments[0].DeveloperNames, resultDto.DeveloperNames);
        Assert.Equal(fakeIndexDocuments[0].PriceAmount, resultDto.PriceAmount);

        gameSearchRepositoryMock.Verify(x => x.SearchAsync(decodedQuery), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoGamesFound_ShouldReturnEmptyList()
    {
        // Arrange
        var gameSearchRepositoryMock = new Mock<IGameSearchRepository>(MockBehavior.Strict);
        var loggerMock = new Mock<ILogger<SearchGameUseCase>>(MockBehavior.Loose);
        
        var sut = new SearchGameUseCase(gameSearchRepositoryMock.Object, loggerMock.Object);

        string query = "jogo-inexistente";

        gameSearchRepositoryMock
            .Setup(x => x.SearchAsync(query))
            .ReturnsAsync(new List<GameIndexDocument>());

        var request = new SearchGameRequest { Query = query };

        // Act
        var response = await sut.ExecuteAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.SearchResult);
        Assert.Empty(response.SearchResult);

        gameSearchRepositoryMock.Verify(x => x.SearchAsync(query), Times.Once);
    }
}