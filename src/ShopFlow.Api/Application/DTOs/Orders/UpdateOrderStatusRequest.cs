using ShopFlow.Api.Domain.Orders.Models;

namespace ShopFlow.Api.Application.DTOs.Orders;

public sealed record UpdateOrderStatusRequest(OrderStatus OrderStatus);