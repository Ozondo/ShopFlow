using MediatR;
using ShopFlow.OrderService.Domain.Orders.Models;
using ShopFlow.OrderService.Infrastructure.Interfaces;

namespace ShopFlow.OrderService.Usecase.GetOrder;

public class GetOrderHandler(IOrdersRepository ordersRepository) : IRequestHandler<GetOrderQuery, Order?>
{
    public async Task<Order?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        return await ordersRepository.GetById(request.Id);
    }
}