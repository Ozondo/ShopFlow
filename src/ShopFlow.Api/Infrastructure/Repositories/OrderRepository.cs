using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Orders.Models;

namespace ShopFlow.Api.Infrastructure.Repositories;

public class OrderRepository(IJsonFileStore jsonFileStore, string ordersPath): IOrdersRepository
{
    private static readonly SemaphoreSlim _lock = new(1, 1);
    
    public async Task<List<Order>?> GetAll()
    {
        var result = await jsonFileStore.ReadAsync<Order>(ordersPath);
        
        return result;
    }

    public async Task<Order?> GetById(Guid id)
    {
        var orders = await jsonFileStore.ReadAsync<Order>(ordersPath);
        
        return orders.FirstOrDefault(x => x.Id == id);
    }

    public async Task<Order> Create(Order order)
    {
        await _lock.WaitAsync();

        try
        {
            var orders = await jsonFileStore.ReadAsync<Order>(ordersPath);
        
            orders.Add(order);
        
            await jsonFileStore.WriteAsync(ordersPath, orders);
        
            return order;
        }
        
        finally
        {
            _lock.Release();
        }

    }
    
    public async Task<Order?> Update(Guid id, OrderStatus orderStatus)
    {
        await _lock.WaitAsync();

        try
        {

            var orders = await jsonFileStore.ReadAsync<Order>(ordersPath);

            var order = orders.FirstOrDefault(x => x.Id == id);

            if (order == null) return null;

            var orderIndex = orders.IndexOf(order);

            var updateOrder = order with { Status = orderStatus };

            orders[orderIndex] = updateOrder;

            await jsonFileStore.WriteAsync(ordersPath, orders);

            return updateOrder;
        }
        
        finally
        {
            _lock.Release();
        }
    }
}