using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;

namespace ShopFlow.ProductService.Usecase.ListProducts;

public sealed record ListProductsQuery : IRequest<List<Product>?>;