using MediatR;
using ShopFlow.OrderService.Domain.Orders.Models;
using ShopFlow.OrderService.Infrastructure.Interfaces;

namespace ShopFlow.OrderService.Usecase.GetAll;

public class GetAllHandler(IOrdersRepository ordersRepository) : IRequestHandler<GetAllQuery, List<Order>?>
{
    public async Task<List<Order>?> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        return await ordersRepository.GetAll();
    }
}