using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;

namespace ShopFlow.ProductService.Usecase.GetByIds;

public sealed record GetByIdsQuery(List<Guid> Ids) : IRequest<List<Product>?>;