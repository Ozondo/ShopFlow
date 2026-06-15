using ShopFlow.Api.Application.Common;
using ShopFlow.Api.Application.DTOs.Products;
using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Products.Models;
using ShopFlow.Api.Infrastructure.Interfaces;

namespace ShopFlow.Api.Domain.Services;

public class ProductService(IProductRepository productRepository) : IProductSevice
{
    public async Task<Result<IReadOnlyList<Product>>> GetAll()
    {
        var products = await productRepository.GetAll();
        
        return Result<IReadOnlyList<Product>>.Ok(products);
    }

    public async Task<Result<Product>> GetById(Guid id)
    {
        var product = await productRepository.GetById(id);
        
        if (product is null) return Result<Product>.Fail($"Product not found");
        
        return Result<Product>.Ok(product);
    }

    public async Task<Result<Product>> Create(CreateProductRequest request)
    {
        if (request.Stock < 1) return Result<Product>.Fail($"Stock must be greater than or equal to 1.");
        if (request.Price <= 0) return  Result<Product>.Fail($"Price must be greater than 0.");
        if (string.IsNullOrEmpty(request.Name)) return  Result<Product>.Fail($"Product name is required.");
        if (string.IsNullOrEmpty(request.Category)) return  Result<Product>.Fail($"Product category is required.");
        
        var product = new Product(
            Guid.NewGuid(),
            request.Name,
            request.Category,
            request.Price,
            request.Stock
        );
        
        var result = await productRepository.Create(product);
        
        return Result<Product>.Ok(result);
    }
    

    public async Task<Result<Product>> Update(Guid id, UpdateProductRequest request)
    {
        var existingProduct = await productRepository.GetById(id);

        if (existingProduct == null)
            return Result<Product>.Fail($"Product with id {id} not found");
        
        if (request.Stock < 1) return Result<Product>.Fail($"Stock must be greater than or equal to 1.");
        if (request.Price <= 0) return Result<Product>.Fail($"Price must be greater than 0.");
        if (string.IsNullOrEmpty(request.Name)) return Result<Product>.Fail($"Product name is required.");
        if (string.IsNullOrEmpty(request.Category)) return Result<Product>.Fail($"Product category is required.");
        
        var product = new Product(
            id,
            request.Name,
            request.Category,
            request.Price,
            request.Stock
        );
        
        var result = await productRepository.Update(product);
        
        return Result<Product>.Ok(product);
    }

    public async Task<Result<Product>> Delete(Guid id)
    {
        var existingProduct = await productRepository.GetById(id);

        if (existingProduct == null)
            return Result<Product>.Fail($"Product with id {id} not found");
        
        var result = await productRepository.Delete(id);
        
        return Result<Product>.Ok(result);
    }
}