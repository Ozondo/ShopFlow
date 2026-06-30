using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Usecase.GetProduct;

public class GetProductHandler(IProductRepository productRepository) : IRequestHandler<GetProductQuery, Product?>
{
    public async Task<Product?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        return await productRepository.GetById(request.Id);
    }
}