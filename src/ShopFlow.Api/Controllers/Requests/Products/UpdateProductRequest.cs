using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Api.Application.DTOs.Products;

public sealed record UpdateProductRequest(
    [Required] string Name,
    [Required] string Category,
    decimal Price,
    int Stock);