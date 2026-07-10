using br.com.fiap.cloudgames.Catalog.Application.Tests.TestData;
using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.CreateGame;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.Tests.UseCases.Game
{
    public class CreateGameUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenAgeRatingIsInvalid_ShouldThrow()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<CreateGameUseCase>>(MockBehavior.Loose);
            var sut = new CreateGameUseCase(uow.Object, repo.Object, logger.Object);

            var request = ApplicationTestData.ValidCreateGameRequest();
            request.AgeRating = "INVALID";

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
            Assert.Contains("Invalid age rating", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_WhenAnyGameModeIsInvalid_ShouldThrow()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<CreateGameUseCase>>(MockBehavior.Loose);
            var sut = new CreateGameUseCase(uow.Object, repo.Object, logger.Object);

            var request = ApplicationTestData.ValidCreateGameRequest();
            request.GameModes = ["INVALID"];

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
            Assert.Contains("Invalid game mode", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_WhenPriceIsInvalid_ShouldThrow()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<CreateGameUseCase>>(MockBehavior.Loose);
            var sut = new CreateGameUseCase(uow.Object, repo.Object, logger.Object);

            var request = ApplicationTestData.ValidCreateGameRequest();
            request.Price = -1;

            var ex = await Assert.ThrowsAsync<br.com.fiap.cloudgames.Catalog.Domain.Exceptions.DomainException>(() => sut.ExecuteAsync(request));
            Assert.Contains("Price can't be negative.", ex.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidRequest_ShouldPersistAndReturnResponse()
        {
            var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var repo = new Mock<IGameRepository>(MockBehavior.Strict);
            var logger = new Mock<ILogger<CreateGameUseCase>>(MockBehavior.Loose);

            repo.Setup(r => r.AddAsync(It.IsAny<br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game>()))
                .Returns(Task.CompletedTask);
            uow.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            var sut = new CreateGameUseCase(uow.Object, repo.Object, logger.Object);
            var request = ApplicationTestData.ValidCreateGameRequest();

            var response = await sut.ExecuteAsync(request);

            Assert.False(string.IsNullOrWhiteSpace(response.Id));
            Assert.Equal(request.Title, response.Title);
            Assert.Equal(request.Description, response.Description);
            Assert.Equal(request.Story, response.Story);
            Assert.Equal(request.Franchise, response.Franchise);
            Assert.Equal(request.ReleaseDate, response.ReleaseDate);
            Assert.Equal(request.AgeRating, response.AgeRating);
            Assert.Contains("Dev Studio", response.Developers);
            Assert.Contains("SinglePlayer", response.GameModes);
            Assert.Contains("Publisher Inc", response.Publisher);

            repo.Verify(r => r.AddAsync(It.IsAny<br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game>()), Times.Once);
            uow.Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}
