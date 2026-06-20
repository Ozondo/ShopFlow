using ShopFlow.Api.Domain.Orders.Models;

namespace ShopFlow.OrderService.Domain.Orders.Models;

public sealed record OrderDTO(
    Guid Id,
    string CustomerName,
    IReadOnlyList<OrderItemDTO> Items,
    OrderStatus Status,
    DateTimeOffset CreatedAt);
