using ShopFlow.Api.Application.Common;
using ShopFlow.Api.Application.DTOs.Orders;
using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Orders.Models;

namespace ShopFlow.Api.Application.Services;

public class OrderService: IOrderService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IProductRepository _productRepository;
    
    public OrderService(IOrdersRepository ordersRepository, IProductRepository productRepository)
    {
        _ordersRepository = ordersRepository;
        _productRepository = productRepository;
    }
    
    public async Task<Result<IReadOnlyList<Order>>> GetAll()
    {
        var orders = await _ordersRepository.GetAll();
        
        if (orders == null) return Result<IReadOnlyList<Order>>.Fail("Orders not found");
        
        return Result<IReadOnlyList<Order>>.Ok(orders);
    }

    public async Task<Result<Order>> GetById(Guid id)
    {
        var order = await _ordersRepository.GetById(id);
        
        if (order == null) return Result<Order>.Fail($"Order with id {id} not found");
        
        return Result<Order>.Ok(order);
    }

    public async Task<Result<Order>> Create(CreateOrderRequest request)
    {
        
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetById(item.ProductId);
            
            if (product == null) return Result<Order>.Fail($"Product with id {item.ProductId} not found");
            if (product.Stock == 0) return Result<Order>.Fail($"Product with id {item.ProductId} stock is 0");
            if (product.Stock < item.Quantity) return Result<Order>.Fail($"Product with id {item.ProductId} not enough stock");
            
            orderItems.Add(new OrderItem(
                product.Id,
                product.Name,
                item.Quantity,
                product.Price
            ));
        }

        var order = new Order(
            Guid.NewGuid(),
            request.CustomerName,
            orderItems,
            OrderStatus.New,
            DateTimeOffset.UtcNow
        );
        
        var result = await _ordersRepository.Create(order);
        
        return Result<Order>.Ok(result);
    }

    public async Task<Result<Order>> Update(Guid id, UpdateOrderStatusRequest request)
    {
        var order = await _ordersRepository.GetById(id);
        
        if (order == null) return Result<Order>.Fail($"Order with id {id} not found");
        
        if (order.Status == request.OrderStatus)
            return Result<Order>.Fail($"Order already has status {request.OrderStatus}");
        
        if (order.Status == OrderStatus.Shipped) 
            return Result<Order>.Fail($"Order with id {id} is shipped");
        
        if (order.Status == OrderStatus.Cancelled)
            return Result<Order>.Fail($"Order with id {id} is cancelled");
        
        if (order.Status == OrderStatus.New && request.OrderStatus == OrderStatus.Shipped)
            return Result<Order>.Fail($"Order with id {id} can have status Processing or Cancelled");
        
        if (order.Status == OrderStatus.Processing && request.OrderStatus == OrderStatus.New)
            return Result<Order>.Fail($"Order with id {id} can`t have status New");
        
        var result = await _ordersRepository.Update(id, request.OrderStatus);
        
        if (result == null)
            return Result<Order>.Fail($"Order with id {id} not found");
        
        return Result<Order>.Ok(result);
    }
}