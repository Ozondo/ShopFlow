using MediatR;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;
using ShopFlow.ProductService.Usecase.GetProduct;

namespace ShopFlow.ProductService.Usecase.CreateProduct;

public class CreateProductHandler(IProductRepository productRepository) :  IRequestHandler<CreateProductCommand, Product>
{
    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        return await productRepository.Create(new Product(
            Id: Guid.NewGuid(),
            Name: request.Name,
            Category: request.Category,
            Price: request.Price, 
            Stock: request.Stock
            ));
    }
}