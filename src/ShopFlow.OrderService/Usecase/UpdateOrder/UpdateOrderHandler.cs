using MediatR;
using ShopFlow.Common.Redis;
using ShopFlow.OrderService.Domain.Orders.Models;
using ShopFlow.OrderService.Infrastructure.Interfaces;

namespace ShopFlow.OrderService.Usecase.UpdateOrder;

public class UpdateOrderHandler(IOrdersRepository ordersRepository, ICacheService cacheService) : IRequestHandler<UpdateOrderCommand, Order?>
{
    public async Task<Order?> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetById(request.Id);
        
        if (order == null) return null;
        
        var newOrder = new Order(request.Id, order.CustomerName, order.Items, (OrderStatus)request.Status, order.CreatedAt);
        
        var result =  await ordersRepository.Update(newOrder);
        
        var cacheKey = $"order:{request.Id}";
        await cacheService.RemoveAsync(cacheKey);
        
        return result;
    }
}