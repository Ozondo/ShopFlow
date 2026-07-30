using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Usecase.UpdateProduct;

public class UpdateProductHandler(IProductRepository productRepository) : IRequestHandler<UpdateProductCommand, Product?>
{
    public async Task<Product?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return await productRepository.Update(new Product(request.Id, request.Name, request.Category, request.Price, request.Stock));
    }
}