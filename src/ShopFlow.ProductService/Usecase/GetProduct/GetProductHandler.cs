using MediatR;
using ShopFlow.Common.Redis;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Usecase.GetProduct;

public class GetProductHandler(IProductRepository productRepository, ICacheService cacheService) : IRequestHandler<GetProductQuery, Product?>
{
    public async Task<Product?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"product:{request.Id}";
        
        var cacheResult = await cacheService.GetAsync<Product>(cacheKey);
        
        if (cacheResult != null)
        {
            return cacheResult;
        }
        
        var mongoResult =  await productRepository.GetById(request.Id);
        
        if (mongoResult == null)
        {
            return null;
        }

        await cacheService.SetAsync(cacheKey, mongoResult, TimeSpan.FromMinutes(5));
        
        return mongoResult;
    }
}