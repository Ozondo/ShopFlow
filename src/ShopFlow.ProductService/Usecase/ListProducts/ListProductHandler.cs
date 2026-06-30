using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Usecase.ListProducts;

public class ListProductHandler(IProductRepository productRepository) : IRequestHandler<ListProductsQuery, List<Product>?>
{
    public async Task<List<Product>?> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        return await productRepository.GetAll();
    }
}