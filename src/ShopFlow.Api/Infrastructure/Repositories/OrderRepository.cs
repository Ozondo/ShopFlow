using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Api.Domain.Products.Models;

namespace ShopFlow.Api.Infrastructure.Repositories;

public class OrderRepository(JsonFileStore jsonFileStore, string ordersPath): IOrdersRepository
{
    private readonly JsonFileStore  _jsonFileStore = jsonFileStore;
    private readonly string _ordersPath = ordersPath;
    
    public async Task<List<Order>> GetAll()
    {
        var result = await _jsonFileStore.ReadAsync<Order>(_ordersPath);
        
        return result;
    }

    public async Task<Order?> GetById(Guid id)
    {
        var orders = await _jsonFileStore.ReadAsync<Order>(_ordersPath);
        
        return orders.FirstOrDefault(x => x.Id == id);
    }

    public async Task<Order> Create(Order order)
    {
        var orders = await _jsonFileStore.ReadAsync<Order>(_ordersPath);
        
        orders.Add(order);
        
        await _jsonFileStore.WriteAsync(_ordersPath, orders);
        
        return order;
    }
    
    public async Task<Order?> Update(Guid id, OrderStatus orderStatus)
    {
        var orders = await _jsonFileStore.ReadAsync<Order>(_ordersPath);
        
        var order = orders.FirstOrDefault(x => x.Id == id);
        
        if (order == null) return null;
        
        var orderIndex = orders.IndexOf(order);
        
        var updateOrder = order with { Status = orderStatus };
        
        orders[orderIndex] = updateOrder;
        
        await _jsonFileStore.WriteAsync(_ordersPath, orders);
        
        return updateOrder;
    }
}