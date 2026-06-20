using ShopFlow.Contracts.Order.V1;
using ShopFlow.OrderService.Domain.Orders.Models;
using ShopFlow.OrderService.Infrastructure.Repositories;
using ShopFlow.Api.Domain.Orders.Models;

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
        var orders = new List<OrderDTO>
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
            new List<OrderDTO> { order });

        var result = await _repository.GetById(order.Id);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        await _store.WriteAsync(
            _path,
            new List<OrderDTO>());

        var result = await _repository.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ShouldAddOrder()
    {
        var order = CreateOrder();

        await _store.WriteAsync(
            _path,
            new List<OrderDTO>());

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
            new List<OrderDTO> { order });

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

    private static OrderDTO CreateOrder(
        OrderStatus status = OrderStatus.New)
    {
        return new OrderDTO(
            Guid.NewGuid(),
            "Test Customer",
            new List<OrderItemDTO>(),
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