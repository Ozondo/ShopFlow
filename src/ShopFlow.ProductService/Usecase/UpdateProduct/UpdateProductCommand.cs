using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;

namespace ShopFlow.ProductService.Usecase.UpdateProduct;

public sealed record UpdateProductCommand(Guid Id ,string Name, string Category, decimal Price, int Stock): IRequest<Product>;