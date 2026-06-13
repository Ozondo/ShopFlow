using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Products.Models;

namespace ShopFlow.Api.Infrastructure.Repositories;

public class ProductRepository(IJsonFileStore jsonFileStore, string productPath): IProductRepository
{
    private static readonly SemaphoreSlim _lock = new(1, 1);
    
    public async Task<List<Product>?> GetAll()
    {
        var products = await jsonFileStore.ReadAsync<Product>(productPath);
        
        return products;
    }

    public async Task<Product?> GetById(Guid id)
    {
        var products = await GetAll();
        
        if (products == null) return null;
        
        return products.FirstOrDefault(x => x.Id == id);
    }
    
    public async Task<Product> Create(Product product)
    {
        await _lock.WaitAsync();

        try
        {

            var products = await GetAll();

            products.Add(product);

            await jsonFileStore.WriteAsync(productPath, products);

            return product;
        }
        
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Product?> Update(Guid id, Product product)
    {
        await _lock.WaitAsync();

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
            _lock.Release();
        }
    }

    public async Task<Product?> Delete(Guid id)
    {
        await _lock.WaitAsync();

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
            _lock.Release();
        }
    }
}