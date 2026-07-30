using MediatR;
using ShopFlow.OrderService.Domain.Orders.Models;

namespace ShopFlow.OrderService.Usecase.GetOrder;

public sealed record GetOrderQuery(Guid Id) : IRequest<Order?>;
