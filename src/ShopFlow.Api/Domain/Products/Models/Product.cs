namespace ShopFlow.Api.Domain.Products.Models;

public sealed record Product(
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    int Stock);
