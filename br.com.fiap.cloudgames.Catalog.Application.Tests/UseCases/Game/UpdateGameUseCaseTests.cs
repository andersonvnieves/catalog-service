using br.com.fiap.cloudgames.Catalog.Application.Tests.TestData;
using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.UpdateGame;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.Tests.UseCases.Game
{
    public class UpdateGameUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenGameIdIsInvalid_ShouldThrow()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<UpdateGameUseCase>>(MockBehavior.Loose);
            var sut = new UpdateGameUseCase(uow.Object, repo.Object, logger.Object);

            var request = ApplicationTestData.ValidUpdateGameRequest();
            request.Id = "not-a-guid";

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
            Assert.Contains("Invalid game id", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_WhenAgeRatingIsInvalid_ShouldThrow()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<UpdateGameUseCase>>(MockBehavior.Loose);
            var sut = new UpdateGameUseCase(uow.Object, repo.Object, logger.Object);

            var request = ApplicationTestData.ValidUpdateGameRequest();
            request.AgeRating = "INVALID";

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
            Assert.Contains("Invalid age rating", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_WhenAnyGameModeIsInvalid_ShouldThrow()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<UpdateGameUseCase>>(MockBehavior.Loose);
            var sut = new UpdateGameUseCase(uow.Object, repo.Object, logger.Object);

            var request = ApplicationTestData.ValidUpdateGameRequest();
            request.GameModes = ["INVALID"];

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
            Assert.Contains("Invalid game mode", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_WhenAnyPlatformIsInvalid_ShouldThrow()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<UpdateGameUseCase>>(MockBehavior.Loose);
            var sut = new UpdateGameUseCase(uow.Object, repo.Object, logger.Object);

            var request = ApplicationTestData.ValidUpdateGameRequest();

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
            Assert.Contains("Invalid platform", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_WhenGameDoesNotExist_ShouldThrow()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<UpdateGameUseCase>>(MockBehavior.Loose);
            var sut = new UpdateGameUseCase(uow.Object, repo.Object, logger.Object);

            var request = ApplicationTestData.ValidUpdateGameRequest();
            var gameId = Guid.Parse(request.Id);

            repo.Setup(x => x.GetByIdAsync(gameId)).ReturnsAsync((br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game?)null);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidRequest_ShouldUpdatePersistAndReturnResponse()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<UpdateGameUseCase>>(MockBehavior.Loose);

            var existingGame = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game.CreateGame(
                "My Game",
                "A cool game",
                "Once upon a time",
                "Franchise X",
                DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                AgeRating.LIVRE,
                new List<GameModes> { GameModes.SinglePlayer },
                new Publisher("Publisher Inc"),
                new List<Developer> { new Developer("Dev Studio") },
                new Price(10));

            var request = ApplicationTestData.ValidUpdateGameRequest();
            request.Id = existingGame.Id.ToString();

            repo.Setup(x => x.GetByIdAsync(existingGame.Id)).ReturnsAsync(existingGame);
            repo.Setup(x => x.Update(existingGame));
            uow.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            var sut = new UpdateGameUseCase(uow.Object, repo.Object, logger.Object);

            var response = await sut.ExecuteAsync(request);

            Assert.Equal(existingGame.Id.ToString(), response.Id);
            Assert.Equal(request.Title, response.Title);
            Assert.Equal(request.Description, response.Description);
            Assert.Equal(request.Story, response.Story);
            Assert.Equal(request.Franchise, response.Franchise);
            Assert.Equal(request.ReleaseDate, response.ReleaseDate);
            Assert.Equal(request.AgeRating, response.AgeRating);
            Assert.Contains("Dev Studio", response.Developers);
            Assert.Contains("SinglePlayer", response.GameModes);
            Assert.Contains("Publisher Inc", response.Publisher);

            repo.Verify(x => x.GetByIdAsync(existingGame.Id), Times.Once);
            repo.Verify(x => x.Update(existingGame), Times.Once);
            uow.Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}
