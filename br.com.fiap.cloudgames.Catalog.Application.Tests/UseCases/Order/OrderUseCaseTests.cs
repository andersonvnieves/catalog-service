using br.com.fiap.cloudgames.Catalog.Application.Abstractions;
using br.com.fiap.cloudgames.Catalog.Application.Publishers;
using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CancelOrder;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CompleteOrder;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CreateOrder;
using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;
using Moq;

namespace br.com.fiap.cloudgames.Catalog.Application.Tests.UseCases.Order;

public class OrderUseCaseTests
{
    [Fact]
    public async Task CreateOrder_WithAvailableGames_ShouldPersistCommitAndPublishPaymentEvent()
    {
        var userId = Guid.NewGuid();
        var game = CreateGame("Game A", 39.90m);
        var orders = new Mock<IOrderRepository>(MockBehavior.Strict);
        var games = new Mock<IGameRepository>(MockBehavior.Strict);
        var libraries = new Mock<ILibraryRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var publisher = new Mock<IOrderCreatedEventPublisher>(MockBehavior.Strict);
        var currentUser = CurrentUser(userId);
        var request = new CreateOrderRequest { GameIds = [game.Id.ToString()] };

        games.Setup(x => x.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids => ids.Single() == game.Id))).ReturnsAsync([game]);
        libraries.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library?)null);
        unitOfWork.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        orders.Setup(x => x.AddAsync(It.IsAny<br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Order>())).Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
        publisher.Setup(x => x.PublishAsync(It.Is<br.com.fiap.cloudgames.Catalog.Application.Events.OrderCreatedEvent>(e =>
            e.UserId == userId && e.TotalAmount == 39.90m && e.OrderId != Guid.Empty))).Returns(Task.CompletedTask);

        var response = await new CreateOrderUseCase(orders.Object, games.Object, libraries.Object, unitOfWork.Object, publisher.Object, currentUser.Object).ExecuteAsync(request);

        Assert.NotEqual(Guid.Empty.ToString(), response.OrderId);
        Assert.Equal(39.90m, response.TotalAmount);
        Assert.Equal(39.90m, response.Items[game.Title]);
        unitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
        orders.Verify(x => x.AddAsync(It.IsAny<br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Order>()), Times.Once);
        unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        publisher.Verify(x => x.PublishAsync(It.IsAny<br.com.fiap.cloudgames.Catalog.Application.Events.OrderCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidOrUnavailableGames_ShouldNotStartTransaction()
    {
        var orders = new Mock<IOrderRepository>(MockBehavior.Strict);
        var games = new Mock<IGameRepository>(MockBehavior.Strict);
        var libraries = new Mock<ILibraryRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var publisher = new Mock<IOrderCreatedEventPublisher>(MockBehavior.Strict);
        var currentUser = CurrentUser(Guid.NewGuid());

        var invalidIdException = await Assert.ThrowsAsync<ApplicationException>(() =>
            new CreateOrderUseCase(orders.Object, games.Object, libraries.Object, unitOfWork.Object, publisher.Object, currentUser.Object)
                .ExecuteAsync(new CreateOrderRequest { GameIds = ["not-a-guid"] }));
        Assert.Equal("Invalid game id", invalidIdException.Message);

        var gameId = Guid.NewGuid();
        games.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync([]);
        var missingGameException = await Assert.ThrowsAsync<ApplicationException>(() =>
            new CreateOrderUseCase(orders.Object, games.Object, libraries.Object, unitOfWork.Object, publisher.Object, currentUser.Object)
                .ExecuteAsync(new CreateOrderRequest { GameIds = [gameId.ToString()] }));
        Assert.Equal("One or more games not found", missingGameException.Message);
        unitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Never);
    }

    [Theory]
    [InlineData(true, OrderStatus.Paid)]
    [InlineData(false, OrderStatus.Cancelled)]
    public async Task CompleteOrCancelOrder_WhenPending_ShouldUpdateStatusAndCommit(bool complete, OrderStatus expectedStatus)
    {
        var order = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Order.Create(Guid.NewGuid(), [new OrderItem(Guid.NewGuid(), new Price(10))]);
        var repository = new Mock<IOrderRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        repository.Setup(x => x.GetByIdAsync(order.Id)).ReturnsAsync(order);
        unitOfWork.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        if (complete)
            await new CompleteOrderUseCase(repository.Object, unitOfWork.Object).ExecuteAsync(order.Id);
        else
            await new CancelOrderUseCase(repository.Object, unitOfWork.Object).ExecuteAsync(order.Id);

        Assert.Equal(expectedStatus, order.OrderStatus);
        unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
    }

    private static Mock<ICurrentUser> CurrentUser(Guid userId)
    {
        var mock = new Mock<ICurrentUser>(MockBehavior.Strict);
        mock.SetupGet(x => x.UserId).Returns(userId);
        mock.SetupGet(x => x.Name).Returns("Player One");
        mock.SetupGet(x => x.Email).Returns("player@example.com");
        return mock;
    }

    private static br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game CreateGame(string title, decimal price) => br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game.CreateGame(title, "Description", "Story", "Franchise",
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), AgeRating.LIVRE, [GameModes.SinglePlayer],
        new Publisher("Publisher"), [new Developer("Developer")], new Price(price));
}
