using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Usecase.DeleteProduct;

public class DeleteProductHandler(IProductRepository productRepository) : IRequestHandler<DeleteProductCommand, Product?> 
{
    public async Task<Product?> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        return await productRepository.Delete(request.Id);
    }
}