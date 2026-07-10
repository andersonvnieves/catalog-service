using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Exceptions;
using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Catalog.Domain.Tests.Aggregates;

public class OrderTests
{
    [Fact]
    public void Create_WithDistinctItems_ShouldCreatePendingOrderAndCalculateTotal()
    {
        var userId = Guid.NewGuid();
        var order = Order.Create(userId,
        [
            new OrderItem(Guid.NewGuid(), new Price(20.50m)),
            new OrderItem(Guid.NewGuid(), new Price(9.49m))
        ]);

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(userId, order.UserId);
        Assert.Equal(OrderStatus.Pending, order.OrderStatus);
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(29.99m, order.TotalAmount.PriceValue);
    }

    [Fact]
    public void Create_WithEmptyUserOrDuplicateGames_ShouldReportValidationErrors()
    {
        var gameId = Guid.NewGuid();
        var exception = Assert.Throws<DomainException>(() => Order.Create(Guid.Empty,
        [new OrderItem(gameId, new Price(10)), new OrderItem(gameId, new Price(10))]));

        Assert.Contains("UserId is required.", exception.Errors);
        Assert.Contains("An order cannot contain duplicate games.", exception.Errors);
    }

    [Fact]
    public void OrderHasBeenPaid_WhenPending_ShouldMarkAsPaid_AndPreventFurtherProcessing()
    {
        var order = Order.Create(Guid.NewGuid(), [new OrderItem(Guid.NewGuid(), new Price(10))]);

        order.OrderHasBeenPaid();

        Assert.Equal(OrderStatus.Paid, order.OrderStatus);
        var exception = Assert.Throws<DomainException>(order.OrderHasBeenCancelled);
        Assert.Contains("Only pending orders can be cancelled.", exception.Errors);
    }

    [Fact]
    public void OrderHasBeenCancelled_WhenPending_ShouldMarkAsCancelled()
    {
        var order = Order.Create(Guid.NewGuid(), [new OrderItem(Guid.NewGuid(), new Price(10))]);

        order.OrderHasBeenCancelled();

        Assert.Equal(OrderStatus.Cancelled, order.OrderStatus);
    }
}
