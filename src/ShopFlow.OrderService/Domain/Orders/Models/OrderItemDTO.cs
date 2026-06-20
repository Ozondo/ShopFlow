namespace ShopFlow.Api.Domain.Orders.Models;

public sealed record OrderItemDTO(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
