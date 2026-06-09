using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Api.Infrastructure;
using ShopFlow.Api.Infrastructure.Repositories;

namespace ShopFlow.Api.Tests.Unit;

public class OrderRepositoryTests
{
    private Order CreateOrder()
    {
        return new Order(
            Guid.NewGuid(),
            "Иван Иванов",
            new List<OrderItem>
            {
                new OrderItem(
                    Guid.NewGuid(),
                    "iPhone",
                    1,
                    100)
            },
            OrderStatus.New,
            DateTimeOffset.UtcNow);
    }
    
    [Fact]
    public async Task GetAll_WhenDataIsEmpty_ReturnsEmptyList()
    {
        var path = Path.GetTempFileName();

        var repository = new OrderRepository(
            new JsonFileStore(),
            path);
        
        var result = await repository.GetAll();
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetAll_WhenDataIsNotEmpty_ReturnsList()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        await jsonFileStore.WriteAsync(
            path,
            new List<Order>
            {
                CreateOrder(),
                CreateOrder(),
            });
        
        
        var repository = new OrderRepository(
            jsonFileStore,
            path);
        
        var result = await repository.GetAll();
        
        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public async Task GetById_WhenDataIsEmpty_ReturnsNull()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        var repository = new OrderRepository(
            jsonFileStore,
            path);
        
        var result = await repository.GetById(Guid.Empty);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetById_WhenDataIsNotEmpty_ReturnsOrder()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        var repository = new OrderRepository(
            jsonFileStore,
            path);
        
        var order = CreateOrder();
        
        await jsonFileStore.WriteAsync(
            path,
            new List<Order>
            {
                order
            });
        
        var result = await repository.GetById(order.Id);
        
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }
    
    [Fact]
    public async Task Create_WhenOrderIsValid_ReturnsOrder()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        var repository = new OrderRepository(
            jsonFileStore,
            path);
        
        var order = CreateOrder();
        
        await repository.Create(order);
        
        var orders = await repository.GetAll();
        
        Assert.NotNull(orders);
        Assert.Equal(order.Id, orders[0].Id);
    }
    
    [Fact]
    public async Task Update_WhenOrderExists_ChangesStatus()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();

        var repository = new OrderRepository(
            jsonFileStore,
            path);

        var order = CreateOrder();

        await repository.Create(order);

        await repository.Update(
            order.Id,
            OrderStatus.Processing);

        var result = await repository.GetById(order.Id);

        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Processing, result.Status);
    }
    
    [Fact]
    public async Task Update_WhenOrderDoesNotExist_ReturnsNull()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();

        var repository = new OrderRepository(
            jsonFileStore,
            path);

        var order = CreateOrder();

        await repository.Create(order);

        var result = await repository.Update(
            Guid.NewGuid(),
            OrderStatus.Processing);

        Assert.Null(result);
    }
}