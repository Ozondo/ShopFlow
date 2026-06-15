using ShopFlow.Api.Domain.Products.Models;

namespace ShopFlow.Api.Infrastructure.Interfaces;

public interface IProductRepository
{
    public Task<List<Product>?> GetAll();
    public Task<Product?> GetById(Guid id);
    public Task<List<Product>> GetByIds(IEnumerable<Guid> ids);
    public Task<Product> Create(Product product);
    public Task<Product?> Update(Guid id, Product product); 
    public Task<Product?> Delete(Guid id);
}