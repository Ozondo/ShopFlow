namespace ShopFlow.Api.Domain.Orders.Models;

public sealed record OrderItem(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
