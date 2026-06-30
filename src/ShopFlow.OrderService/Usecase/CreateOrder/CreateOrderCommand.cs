using MediatR;
using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.OrderService.Domain.Orders.Models;

namespace ShopFlow.OrderService.Usecase.CreateOrder;

public sealed record CreateOrderCommand(string CustomerName,
    IReadOnlyList<CreateOrderItem> Items) :  IRequest<Order>;

public sealed record CreateOrderItem(
    Guid ProductId,
    int Quantity);