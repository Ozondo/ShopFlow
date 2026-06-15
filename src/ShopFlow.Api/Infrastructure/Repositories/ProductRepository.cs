using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Products.Models;
using ShopFlow.Api.Infrastructure.Interfaces;

namespace ShopFlow.Api.Infrastructure.Repositories;

public class ProductRepository(IJsonFileStore jsonFileStore, string productPath): IProductRepository
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    
    public async Task<List<Product>?> GetAll()
    {
        var products = await jsonFileStore.ReadAsync<Product>(productPath);
        
        return products;
    }

    public async Task<Product?> GetById(Guid id)
    {
        var products = await GetAll();
        
        return products.FirstOrDefault(x => x.Id == id);
    }
    
    public async Task<List<Product>> GetByIds(IEnumerable<Guid> ids)
    {
        var products = await GetAll();
        var idsList = ids.ToHashSet();
        
        return products.Where(x => idsList.Contains(x.Id)).ToList();
    }
    
    public async Task<Product> Create(Product product)
    {
        await Lock.WaitAsync();

        try
        {

            var products = await GetAll();

            products.Add(product);

            await jsonFileStore.WriteAsync(productPath, products);

            return product;
        }
        
        finally
        {
            Lock.Release();
        }
    }

    public async Task<Product?> Update(Guid id, Product product)
    {
        await Lock.WaitAsync();

        try
        {

            var products = await GetAll();

            var existingProduct = products.FirstOrDefault(x => x.Id == id);

            if (existingProduct == null) return null;

            var indexOfExistingProduct = products.IndexOf(existingProduct);

            products[indexOfExistingProduct] = product;

            await jsonFileStore.WriteAsync(productPath, products);

            return product;
        }
        
        finally
        {
            Lock.Release();
        }
    }

    public async Task<Product?> Delete(Guid id)
    {
        await Lock.WaitAsync();

        try
        {
            var products = await GetAll();

            var deleteProduct = products.FirstOrDefault(x => x.Id == id);

            if (deleteProduct == null) return null;

            products.Remove(deleteProduct);

            await jsonFileStore.WriteAsync(productPath, products);

            return deleteProduct;
        }
        
        finally
        {
            Lock.Release();
        }
    }
}