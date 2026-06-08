namespace ShopFlow.Api.Domain.Orders.Models;

public sealed record Order(
    Guid Id,
    string CustomerName,
    IReadOnlyList<OrderItem> Items,
    OrderStatus Status,
    DateTimeOffset CreatedAt);
