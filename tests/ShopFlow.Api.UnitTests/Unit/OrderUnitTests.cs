using Moq;
using ShopFlow.Api.Application.DTOs.Orders;
using ShopFlow.Api.Application.Services;
using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Api.Domain.Products.Models;
using ShopFlow.Api.Infrastructure.Interfaces;

namespace ShopFlow.Api.Tests.Unit;

public class OrderServiceTests
{
    private readonly Mock<IOrdersRepository> _ordersRepositoryMock = new();
    private readonly Mock<IProductRepository> _productRepositoryMock = new();

    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService(
            _ordersRepositoryMock.Object,
            _productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOrders()
    {
        var orders = new List<Order>
        {
            CreateOrder()
        };

        _ordersRepositoryMock
            .Setup(x => x.GetAll())
            .ReturnsAsync(orders);

        var result = await _service.GetAll();

        Assert.True(result.Success);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetById_ShouldReturnOrder_WhenOrderExists()
    {
        var order = CreateOrder();

        _ordersRepositoryMock
            .Setup(x => x.GetById(order.Id))
            .ReturnsAsync(order);

        var result = await _service.GetById(order.Id);

        Assert.True(result.Success);
        Assert.Equal(order.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetById_ShouldFail_WhenOrderNotFound()
    {
        var id = Guid.NewGuid();

        _ordersRepositoryMock
            .Setup(x => x.GetById(id))
            .ReturnsAsync((Order?)null);

        var result = await _service.GetById(id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Create_ShouldCreateOrder_WhenProductsExist()
    {
        var product = CreateProduct();

        _productRepositoryMock
            .Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Product> { product });

        _ordersRepositoryMock
            .Setup(x => x.Create(It.IsAny<Order>()))
            .ReturnsAsync((Order order) => order);

        var request = CreateOrderRequest(product.Id);

        var result = await _service.Create(request);

        Assert.True(result.Success);

        _ordersRepositoryMock.Verify(
            x => x.Create(It.IsAny<Order>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenProductNotFound()
    {
        _productRepositoryMock
            .Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Product>());

        var request = CreateOrderRequest(Guid.NewGuid());

        var result = await _service.Create(request);

        Assert.False(result.Success);

        _ordersRepositoryMock.Verify(
            x => x.Create(It.IsAny<Order>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenStockIsZero()
    {
        var product = CreateProduct(stock: 0);

        _productRepositoryMock
            .Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Product> { product });

        var request = CreateOrderRequest(product.Id);

        var result = await _service.Create(request);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenStockIsLessThanRequested()
    {
        var product = CreateProduct(stock: 1);

        _productRepositoryMock
            .Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Product> { product });

        var request = CreateOrderRequest(product.Id, quantity: 5);

        var result = await _service.Create(request);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenOrderNotFound()
    {
        var id = Guid.NewGuid();

        _ordersRepositoryMock
            .Setup(x => x.GetById(id))
            .ReturnsAsync((Order?)null);

        var result = await _service.Update(
            id,
            new UpdateOrderStatusRequest(OrderStatus.Processing));

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenOrderIsShipped()
    {
        var order = CreateOrder(OrderStatus.Shipped);

        _ordersRepositoryMock
            .Setup(x => x.GetById(order.Id))
            .ReturnsAsync(order);

        var result = await _service.Update(
            order.Id,
            new UpdateOrderStatusRequest(OrderStatus.Cancelled));

        Assert.False(result.Success);

        _ordersRepositoryMock.Verify(
            x => x.Update(It.IsAny<Order>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenOrderIsCancelled()
    {
        var order = CreateOrder(OrderStatus.Cancelled);

        _ordersRepositoryMock
            .Setup(x => x.GetById(order.Id))
            .ReturnsAsync(order);

        var result = await _service.Update(
            order.Id,
            new UpdateOrderStatusRequest(OrderStatus.Processing));

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenNewOrderMovedToShipped()
    {
        var order = CreateOrder(OrderStatus.New);

        _ordersRepositoryMock
            .Setup(x => x.GetById(order.Id))
            .ReturnsAsync(order);

        var result = await _service.Update(
            order.Id,
            new UpdateOrderStatusRequest(OrderStatus.Shipped));

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenProcessingMovedToNew()
    {
        var order = CreateOrder(OrderStatus.Processing);

        _ordersRepositoryMock
            .Setup(x => x.GetById(order.Id))
            .ReturnsAsync(order);

        var result = await _service.Update(
            order.Id,
            new UpdateOrderStatusRequest(OrderStatus.New));

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldUpdateOrder_WhenTransitionIsValid()
    {
        var order = CreateOrder(OrderStatus.New);

        var updatedOrder = order with
        {
            Status = OrderStatus.Processing
        };

        _ordersRepositoryMock
            .Setup(x => x.GetById(order.Id))
            .ReturnsAsync(order);

        _ordersRepositoryMock
            .Setup(x => x.Update(It.IsAny<Order>()))
            .ReturnsAsync(updatedOrder);

        var result = await _service.Update(
            order.Id,
            new UpdateOrderStatusRequest(OrderStatus.Processing));

        Assert.True(result.Success);
        Assert.Equal(OrderStatus.Processing, result.Data.Status);

        _ordersRepositoryMock.Verify(
            x => x.Update(It.IsAny<Order>()),
            Times.Once);
    }

    private static Product CreateProduct(
        int stock = 10,
        decimal price = 100)
    {
        return new Product(
            Guid.NewGuid(),
            "Test Product",
            "Category",
            price,
            stock);
    }

    private static Order CreateOrder(
        OrderStatus status = OrderStatus.New)
    {
        return new Order(
            Guid.NewGuid(),
            "Customer",
            new List<OrderItem>(),
            status,
            DateTimeOffset.UtcNow);
    }

    private static CreateOrderRequest CreateOrderRequest(
        Guid productId,
        int quantity = 1)
    {
        return new CreateOrderRequest(
            "Customer",
            new List<CreateOrderItemRequest>
            {
                new(productId, quantity)
            });
    }
}