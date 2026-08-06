using MediatR;
using ShopFlow.Common.Redis;
using ShopFlow.OrderService.Domain.Orders.Models;
using ShopFlow.OrderService.Infrastructure.Interfaces;

namespace ShopFlow.OrderService.Usecase.GetOrder;

public class GetOrderHandler(IOrdersRepository ordersRepository, ICacheService cacheService) : IRequestHandler<GetOrderQuery, Order?>
{
    public async Task<Order?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{request.Id}";
        var cacheResult = await cacheService.GetAsync<Order>(cacheKey);

        if (cacheResult != null)
        {
            return cacheResult;
        }
        
        var mongoResult = await ordersRepository.GetById(request.Id);

        if (mongoResult == null)
            return null;
        
        await cacheService.SetAsync(cacheKey, mongoResult, TimeSpan.FromMinutes(5));
        
        return mongoResult;
    }
}