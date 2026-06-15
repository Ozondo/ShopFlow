using ShopFlow.Api.Application.Common;
using ShopFlow.Api.Application.DTOs.Orders;
using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Api.Infrastructure.Interfaces;

namespace ShopFlow.Api.Domain.Services;

public class OrderService(IOrdersRepository ordersRepository, IProductRepository productRepository)
    : IOrderService
{
    public async Task<Result<IReadOnlyList<Order>>> GetAll()
    {
        var orders = await ordersRepository.GetAll();
        
        return Result<IReadOnlyList<Order>>.Ok(orders);
    }

    public async Task<Result<Order>> GetById(Guid id)
    {
        var order = await ordersRepository.GetById(id);
        
        if (order == null) return Result<Order>.Fail($"OrderRepository with id {id} not found");
        
        return Result<Order>.Ok(order);
    }

    public async Task<Result<Order>> Create(CreateOrderRequest request)
    {
        
        var orderItems = new List<OrderItem>();
        
        var productsIds = request.Items
            .Select(x => x.ProductId)
            .Distinct()
            .ToList();
        
        var products = await productRepository.GetByIds(productsIds);
        
        var dictionaryProducts = products.ToDictionary(x => x.Id);

        foreach (var item in request.Items)
        {
            if (!dictionaryProducts.TryGetValue(item.ProductId, out var product))
                return Result<Order>.Fail($"Product with id {item.ProductId} not found");
            
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
        
        var result = await ordersRepository.Create(order);
        
        return Result<Order>.Ok(result);
    }

    public async Task<Result<Order>> Update(Guid id, UpdateOrderStatusRequest request)
    {
        var order = await ordersRepository.GetById(id);
        
        if (order == null) return Result<Order>.Fail($"OrderRepository with id {id} not found");
        
        switch (order.Status)
        {
            case OrderStatus.Shipped:
                return Result<Order>.Fail($"OrderRepository with id {id} is shipped");

            case OrderStatus.Cancelled:
                return Result<Order>.Fail($"OrderRepository with id {id} is cancelled");

            case OrderStatus.New when request.OrderStatus == OrderStatus.Shipped:
                return Result<Order>.Fail(
                    $"OrderRepository with id {id} can have status Processing or Cancelled");

            case OrderStatus.Processing when request.OrderStatus == OrderStatus.New:
                return Result<Order>.Fail(
                    $"OrderRepository with id {id} can`t have status New");
        }
        
        var updatedOrder = order with
        {
            Status = request.OrderStatus
        };
        
        var result = await ordersRepository.Update(updatedOrder);
        
        return Result<Order>.Ok(result);
    }
}