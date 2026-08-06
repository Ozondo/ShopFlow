using MediatR;
using ShopFlow.Common.Redis;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Usecase.UpdateProduct;

public class UpdateProductHandler(IProductRepository productRepository, ICacheService cacheService) : IRequestHandler<UpdateProductCommand, Product?>
{
    public async Task<Product?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var result = await productRepository.Update(new Product(request.Id, request.Name, request.Category, request.Price, request.Stock));
        
        var cacheKey = $"product:{request.Id}";
        await cacheService.RemoveAsync(cacheKey);

        return result;
    }
}