using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;

namespace ShopFlow.ProductService.Usecase.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Product>;