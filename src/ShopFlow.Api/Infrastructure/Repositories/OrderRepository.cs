using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Api.Infrastructure.Interfaces;

namespace ShopFlow.Api.Infrastructure.Repositories;

public class Order(IJsonFileStore jsonFileStore, string ordersPath): IOrdersRepository
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    
    public async Task<List<Domain.Orders.Models.Order>> GetAll()
    {
        var result = await jsonFileStore.ReadAsync<Domain.Orders.Models.Order>(ordersPath);
        
        return result;
    }

    public async Task<Domain.Orders.Models.Order?> GetById(Guid id)
    {
        var orders = await jsonFileStore.ReadAsync<Domain.Orders.Models.Order>(ordersPath);
        
        return orders.FirstOrDefault(x => x.Id == id);
    }

    public async Task<Domain.Orders.Models.Order> Create(Domain.Orders.Models.Order order)
    {
        await Lock.WaitAsync();

        try
        {
            var orders = await jsonFileStore.ReadAsync<Domain.Orders.Models.Order>(ordersPath);
        
            orders.Add(order);
        
            await jsonFileStore.WriteAsync(ordersPath, orders);
        
            return order;
        }
        
        finally
        {
            Lock.Release();
        }

    }
    
    public async Task<Domain.Orders.Models.Order> Update(Domain.Orders.Models.Order order)
    {
        await Lock.WaitAsync();

        try
        {
            var orders = await jsonFileStore.ReadAsync<Domain.Orders.Models.Order>(ordersPath);

            var orderIndex = orders.FindIndex(x => x.Id == order.Id);

            orders[orderIndex] = order;

            await jsonFileStore.WriteAsync(ordersPath, orders);

            return order;
        }
        
        finally
        {
            Lock.Release();
        }
    }
}