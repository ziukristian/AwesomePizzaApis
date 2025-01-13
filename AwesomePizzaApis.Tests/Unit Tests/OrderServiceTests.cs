using AwesomePizzaApis.Model;
using AwesomePizzaApis.Services;
using FluentAssertions;

namespace AwesomePizzaApis.Tests;

public class OrderServiceTests
{
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderService = new OrderService();
    }

    [Fact]
    public void FindNextOrder_ShouldReturnNull_WhenOrdersIsNull()
    {
        // Act
        var result = _orderService.FindNextOrder(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindNextOrder_ShouldReturnNull_WhenOrdersIsEmpty()
    {
        // Arrange
        var orders = new List<Order>();

        // Act
        var result = _orderService.FindNextOrder(orders);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindNextOrder_ShouldReturnNull_WhenNoCreatedOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new()
            {
                Status = OrderStatus.InProgress,
                CreatedAt = DateTime.Now,
                PizzaType = "Test",
                CustomerEmail = "test@mail.com",
            },
            new()
            {
                Status = OrderStatus.Finished,
                CreatedAt = DateTime.Now.AddMinutes(-5),
                PizzaType = "Test",
                CustomerEmail = "test@mail.com",
            },
        };

        // Act
        var result = _orderService.FindNextOrder(orders);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindNextOrder_ShouldReturnFirstCreatedOrder_WhenCreatedOrdersExist()
    {
        // Arrange
        var orders = new List<Order>
        {
            new()
            {
                Status = OrderStatus.Finished,
                CreatedAt = DateTime.Now.AddMinutes(-5),
                PizzaType = "Test",
                CustomerEmail = "test@mail.com",
            },
            new()
            {
                Status = OrderStatus.Created,
                CreatedAt = DateTime.Now.AddMinutes(5),
                PizzaType = "Test",
                CustomerEmail = "test@mail.com",
            },
            new()
            {
                Status = OrderStatus.Created,
                CreatedAt = DateTime.Now.AddMinutes(-10),
                PizzaType = "Test",
                CustomerEmail = "test@mail.com",
            },
        };

        // Act
        var result = _orderService.FindNextOrder(orders);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Created);
        result.CreatedAt.Should().BeBefore(orders[1].CreatedAt);
    }

    [Fact]
    public void OrganizeOrders_ShouldReturnEmptyList_WhenOrdersIsNull()
    {
        // Act
        var result = _orderService.OrganizeOrders(null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void OrganizeOrders_ShouldReturnEmptyList_WhenOrdersIsEmpty()
    {
        // Arrange
        var orders = new List<Order>();

        // Act
        var result = _orderService.OrganizeOrders(orders);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void OrganizeOrders_ShouldReturnOrderedList_WhenCreatedOrdersExist()
    {
        // Arrange
        var orders = new List<Order>
        {
            new()
            {
                Status = OrderStatus.Created,
                CreatedAt = DateTime.Now.AddMinutes(3),
                PizzaType = "Test",
                CustomerEmail = "test@mail.com",
            },
            new()
            {
                Status = OrderStatus.Created,
                CreatedAt = DateTime.Now.AddMinutes(1),
                PizzaType = "Test",
                CustomerEmail = "test@mail.com",
            },
            new()
            {
                Status = OrderStatus.Finished,
                CreatedAt = DateTime.Now.AddMinutes(5),
                PizzaType = "Test",
                CustomerEmail = "test@mail.com",
            },
        };

        // Act
        var result = _orderService.OrganizeOrders(orders);

        // Assert
        result.First().CreatedAt.Should().BeBefore(result.Last().CreatedAt);
    }
}
