using ShopFlow.Api.Infrastructure.Interfaces;
using ShopFlow.OrderService.Domain.Orders.Models;
using ShopFlow.OrderService.Infrastructure.Interfaces;

namespace ShopFlow.OrderService.Infrastructure.Repositories;

public class OrderRepository(IJsonFileStore jsonFileStore, string ordersPath): IOrdersRepository
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    
    public async Task<List<OrderDTO>> GetAll()
    {
        var result = await jsonFileStore.ReadAsync<OrderDTO>(ordersPath);
        
        return result;
    }

    public async Task<OrderDTO?> GetById(Guid id)
    {
        var orders = await jsonFileStore.ReadAsync<OrderDTO>(ordersPath);
        
        return orders.FirstOrDefault(x => x.Id == id);
    }

    public async Task<OrderDTO> Create(OrderDTO orderDto)
    {
        await Lock.WaitAsync();

        try
        {
            var orders = await jsonFileStore.ReadAsync<OrderDTO>(ordersPath);
        
            orders.Add(orderDto);
        
            await jsonFileStore.WriteAsync(ordersPath, orders);
        
            return orderDto;
        }
        
        finally
        {
            Lock.Release();
        }

    }
    
    public async Task<OrderDTO> Update(OrderDTO orderDto)
    {
        await Lock.WaitAsync();

        try
        {
            var orders = await jsonFileStore.ReadAsync<OrderDTO>(ordersPath);

            var orderIndex = orders.FindIndex(x => x.Id == orderDto.Id);

            orders[orderIndex] = orderDto;

            await jsonFileStore.WriteAsync(ordersPath, orders);

            return orderDto;
        }
        
        finally
        {
            Lock.Release();
        }
    }
}