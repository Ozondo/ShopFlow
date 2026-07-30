using MediatR;
using ShopFlow.OrderService.Domain.Orders.Models;

namespace ShopFlow.OrderService.Usecase.GetAll;

public sealed record GetAllQuery : IRequest<List<Order>?>;