using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Usecase.GetByIds;

public class GetByIdsHandler(IProductRepository productRepository) : IRequestHandler<GetByIdsQuery, List<Product>?>
{
    public async Task<List<Product>?> Handle(GetByIdsQuery request, CancellationToken cancellationToken)
    {
        return await productRepository.GetByIds(request.Ids);
    }
}