using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopFlow.ProductService.Domain.Products.Models;

namespace ShopFlow.ProductService.Infrastructure.Interfaces;

public interface IProductRepository
{
    public Task<List<Product>?> GetAll();
    public Task<Product?> GetById(Guid id);
    public Task<List<Product>> GetByIds(IEnumerable<Guid> ids);
    public Task<Product> Create(Product product);
    public Task<Product> Update(Product product); 
    public Task<Product> Delete(Guid id);
}