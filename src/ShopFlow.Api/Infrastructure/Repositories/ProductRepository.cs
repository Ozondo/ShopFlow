using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Products.Models;

namespace ShopFlow.Api.Infrastructure.Repositories;

public class ProductRepository(JsonFileStore jsonFileStore, string productPath): IProductRepository
{
    private readonly JsonFileStore  _jsonFileStore = jsonFileStore;
    private readonly string _productsPath = productPath;
    
    public async Task<List<Product>> GetAll()
    {
        var products = await _jsonFileStore.ReadAsync<Product>(_productsPath);
        
        return products;
    }

    public async Task<Product?> GetById(Guid id)
    {
        var products = await GetAll();
        
        return products.FirstOrDefault(x => x.Id == id);
    }
    
    public async Task<Product> Create(Product product)
    {
        var products = await GetAll();
        
        products.Add(product);
        
        await _jsonFileStore.WriteAsync(_productsPath, products);
        
        return product;
    }

    public async Task<Product?> Update(Guid id, Product product)
    {
        var products = await GetAll();
        
        var existingProduct = products.FirstOrDefault(x => x.Id == id);

        if (existingProduct == null) return null;

        var indexOfExistingProduct = products.IndexOf(existingProduct);
        
        products[indexOfExistingProduct] = product;
        
        await _jsonFileStore.WriteAsync(_productsPath, products);
        
        return product;
    }

    public async Task<Product?> Delete(Guid id)
    {
        var products = await GetAll();
        
        var deleteProduct = products.FirstOrDefault(x => x.Id == id);

        if (deleteProduct == null) return null;

        products.Remove(deleteProduct);
        
        await _jsonFileStore.WriteAsync(_productsPath, products);
        
        return deleteProduct;
    }
}