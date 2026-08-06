using MediatR;
using ShopFlow.Common.Redis;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Usecase.DeleteProduct;

public class DeleteProductHandler(IProductRepository productRepository, ICacheService cacheService) : IRequestHandler<DeleteProductCommand, Product?> 
{
    public async Task<Product?> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var result =  await productRepository.Delete(request.Id);

        var cacheKey = $"product:{result.Id}";
        await cacheService.RemoveAsync(cacheKey);
        
        return result;
    }
}