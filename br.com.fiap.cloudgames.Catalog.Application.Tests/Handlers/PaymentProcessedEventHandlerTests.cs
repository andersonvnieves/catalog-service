using br.com.fiap.cloudgames.Catalog.Application.Events;
using br.com.fiap.cloudgames.Catalog.Application.Handlers;
using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace br.com.fiap.cloudgames.Catalog.Application.Tests.Handlers;

public class PaymentProcessedEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenPaymentIsApproved_ShouldPayOrderAndCreateLibraryWithGames()
    {
        var order = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Order.Create(Guid.NewGuid(), [new OrderItem(Guid.NewGuid(), new Price(25))]);
        var game = CreateGame(order.Items.Single().GameId);
        var orders = new Mock<IOrderRepository>(MockBehavior.Strict);
        var libraries = new Mock<ILibraryRepository>(MockBehavior.Strict);
        var games = new Mock<IGameRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        orders.Setup(x => x.GetByIdAsync(order.Id)).ReturnsAsync(order);
        orders.Setup(x => x.UpdateAsync(order)).Returns(Task.CompletedTask);
        games.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync([game]);
        libraries.Setup(x => x.GetByIdAsync(order.UserId)).ReturnsAsync((br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library?)null);
        libraries.Setup(x => x.AddAsync(It.Is<br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library>(l => l.UserId == order.UserId))).Returns(Task.CompletedTask);
        libraries.Setup(x => x.UpdateAsync(It.Is<br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library>(l => l.OwnsGame(game.Id)))).Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        await Handler(orders, libraries, games, unitOfWork).HandleAsync(new PaymentProcessedEvent { OrderId = order.Id, PaymentStatus = PaymentStatus.Approved });

        Assert.Equal(OrderStatus.Paid, order.OrderStatus);
        orders.Verify(x => x.UpdateAsync(order), Times.Once);
        libraries.Verify(x => x.AddAsync(It.IsAny<br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library>()), Times.Once);
        libraries.Verify(x => x.UpdateAsync(It.IsAny<br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library>()), Times.Once);
        unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenPaymentIsRejected_ShouldCancelOrderWithoutChangingLibrary()
    {
        var order = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Order.Create(Guid.NewGuid(), [new OrderItem(Guid.NewGuid(), new Price(25))]);
        var orders = new Mock<IOrderRepository>(MockBehavior.Strict);
        var libraries = new Mock<ILibraryRepository>(MockBehavior.Strict);
        var games = new Mock<IGameRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        orders.Setup(x => x.GetByIdAsync(order.Id)).ReturnsAsync(order);
        orders.Setup(x => x.UpdateAsync(order)).Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        await Handler(orders, libraries, games, unitOfWork).HandleAsync(new PaymentProcessedEvent { OrderId = order.Id, PaymentStatus = PaymentStatus.Rejected });

        Assert.Equal(OrderStatus.Cancelled, order.OrderStatus);
        orders.Verify(x => x.UpdateAsync(order), Times.Once);
        unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
    }

    private static PaymentProcessedEventHandler Handler(Mock<IOrderRepository> orders, Mock<ILibraryRepository> libraries, Mock<IGameRepository> games, Mock<IUnitOfWork> unitOfWork) =>
        new(orders.Object, libraries.Object, games.Object, unitOfWork.Object, Mock.Of<ILogger<PaymentProcessedEventHandler>>());

    private static Game CreateGame(Guid id)
    {
        var game = Game.CreateGame("Game", "Description", "Story", "Franchise", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), AgeRating.LIVRE, [GameModes.SinglePlayer], new Publisher("Publisher"), [new Developer("Developer")], new Price(25));
        typeof(Game).GetProperty(nameof(Game.Id))!.SetValue(game, id);
        return game;
    }
}
