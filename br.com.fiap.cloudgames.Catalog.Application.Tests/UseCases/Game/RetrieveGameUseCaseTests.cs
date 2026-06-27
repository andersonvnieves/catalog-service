using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.RetrieveGame;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.Tests.UseCases.Game
{
    public class RetrieveGameUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenGameIdIsInvalid_ShouldThrow()
        {
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<RetrieveGameUseCase>>(MockBehavior.Loose);
            var sut = new RetrieveGameUseCase(repo.Object, logger.Object);

            var request = new RetrieveGameRequest { GameId = "not-a-guid" };

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
            Assert.Contains("Invalid game id", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_WhenGameDoesNotExist_ShouldThrow()
        {
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<RetrieveGameUseCase>>(MockBehavior.Loose);
            var sut = new RetrieveGameUseCase(repo.Object, logger.Object);

            var gameId = Guid.NewGuid();
            repo.Setup(x => x.GetByIdAsync(gameId)).ReturnsAsync((br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game?)null);

            var request = new RetrieveGameRequest { GameId = gameId.ToString() };

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidRequest_ShouldReturnGame()
        {
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<RetrieveGameUseCase>>(MockBehavior.Loose);
            var sut = new RetrieveGameUseCase(repo.Object, logger.Object);

            var game = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game.CreateGame(
                "My Game",
                "A cool game",
                "Once upon a time",
                "Franchise X",
                DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                AgeRating.LIVRE,
                new List<GameModes> { GameModes.SinglePlayer },
                new Publisher("Publisher Inc"),
                new List<Developer> { new Developer("Dev Studio") });

            repo.Setup(x => x.GetByIdAsync(game.Id)).ReturnsAsync(game);

            var request = new RetrieveGameRequest { GameId = game.Id.ToString() };

            var response = await sut.ExecuteAsync(request);

            Assert.Equal(game.Id.ToString(), response.Id);
            Assert.Equal(game.Title, response.Title);
            Assert.Equal(game.Description, response.Description);
            Assert.Equal(game.Story, response.Story);
            Assert.Equal(game.Franchise, response.Franchise);
            Assert.Equal(game.ReleaseDate, response.ReleaseDate);
            Assert.Equal(game.AgeRating.ToString(), response.AgeRating);
            Assert.Contains("SinglePlayer", response.GameModes);
            Assert.Equal("Publisher Inc", response.Publisher);
            Assert.Contains("Dev Studio", response.Developers);
        }
    }
}
