using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;

namespace ShopFlow.ProductService.Usecase.CreateProduct;

public sealed record CreateProductCommand(
    string Name, 
    string Category,
    decimal Price,
    int Stock) : IRequest<Product>;
