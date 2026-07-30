using MediatR;
using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.OrderService.Domain.Orders.Models;
using ShopFlow.OrderService.Infrastructure.Interfaces;

namespace ShopFlow.OrderService.Usecase.UpdateOrder;

public class UpdateOrderHandler(IOrdersRepository ordersRepository) : IRequestHandler<UpdateOrderCommand, Order?>
{
    public async Task<Order?> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetById(request.Id);
        
        if (order == null) return null;
        
        var newOrder = new Order(request.Id, order.CustomerName, order.Items, (OrderStatus)request.Status, order.CreatedAt);
        
        return await ordersRepository.Update(newOrder);
    }
}