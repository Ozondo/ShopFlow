using MediatR;
using ShopFlow.OrderService.Domain.Orders.Models;

namespace ShopFlow.OrderService.Usecase.UpdateOrder;

public sealed record UpdateOrderCommand(Guid Id, OrderStatus Status) : IRequest<Order?>;
