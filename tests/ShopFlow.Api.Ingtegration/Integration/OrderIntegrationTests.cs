using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Api.Infrastructure;
using ShopFlow.Api.Infrastructure.Repositories;

namespace ShopFlow.Api.Tests.Integration;

public class OrderRepositoryTests : IDisposable
{
    private readonly string _path;
    private readonly JsonFileStore _store;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        _path = Path.GetTempFileName();

        _store = new JsonFileStore();

        _repository = new OrderRepository(
            _store,
            _path);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllOrders()
    {
        var orders = new List<Order>
        {
            CreateOrder(),
            CreateOrder()
        };

        await _store.WriteAsync(_path, orders);

        var result = await _repository.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ShouldReturnOrder_WhenOrderExists()
    {
        var order = CreateOrder();

        await _store.WriteAsync(
            _path,
            new List<Order> { order });

        var result = await _repository.GetById(order.Id);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        await _store.WriteAsync(
            _path,
            new List<Order>());

        var result = await _repository.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ShouldAddOrder()
    {
        var order = CreateOrder();

        await _store.WriteAsync(
            _path,
            new List<Order>());

        await _repository.Create(order);

        var result = await _repository.GetAll();

        Assert.Single(result);
        Assert.Equal(order.Id, result[0].Id);
    }

    [Fact]
    public async Task Update_ShouldUpdateOrderStatus()
    {
        var order = CreateOrder();

        await _store.WriteAsync(
            _path,
            new List<Order> { order });

        var updatedOrder = order with
        {
            Status = OrderStatus.Processing
        };

        await _repository.Update(updatedOrder);

        var result = await _repository.GetById(order.Id);

        Assert.NotNull(result);
        Assert.Equal(
            OrderStatus.Processing,
            result!.Status);
    }

    private static Order CreateOrder(
        OrderStatus status = OrderStatus.New)
    {
        return new Order(
            Guid.NewGuid(),
            "Test Customer",
            new List<OrderItem>(),
            status,
            DateTimeOffset.UtcNow);
    }

    public void Dispose()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }
}