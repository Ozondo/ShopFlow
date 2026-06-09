using ShopFlow.Api.Domain.Products.Models;

namespace ShopFlow.Api.Application.Interfaces;

public interface IProductRepository
{
    public Task<List<Product>> GetAll();
    public Task<Product?> GetById(Guid id);
    public Task<Product> Create(Product product);
    public Task<Product?> Update(Guid id, Product product); 
    public Task<Product?> Delete(Guid id);
}