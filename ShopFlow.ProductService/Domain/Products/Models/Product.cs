using System;

namespace ShopFlow.ProductService.Domain.Products.Models;

public sealed record Product(
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    int Stock);
