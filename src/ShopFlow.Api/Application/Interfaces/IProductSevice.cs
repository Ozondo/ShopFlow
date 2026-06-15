using ShopFlow.Api.Application.Common;
using ShopFlow.Api.Application.DTOs.Products;
using ShopFlow.Api.Domain.Products.Models;

namespace ShopFlow.Api.Application.Interfaces;

public interface IProductSevice
{
    public Task<Result<IReadOnlyList<Product>>?> GetAll();
    public Task<Result<Product>> GetById(Guid id);
    public Task<Result<Product>> Create(CreateProductRequest request);
    public Task<Result<Product>> Update(Guid id, UpdateProductRequest request); 
    public Task<Result<Product>> Delete(Guid id);
}