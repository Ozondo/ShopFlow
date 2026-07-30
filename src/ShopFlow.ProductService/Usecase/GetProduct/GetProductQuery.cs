using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;

namespace ShopFlow.ProductService.Usecase.GetProduct;

public sealed record GetProductQuery(Guid Id) : IRequest<Product?>;
