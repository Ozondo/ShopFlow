using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Infrastructure.Repositories;

public class ProductRepository(IJsonFileStore jsonFileStore, string productPath): IProductRepository
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    
    public async Task<List<Product>> GetAll()
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

    public async Task<Product> Update(Product product)
    {
        await Lock.WaitAsync();

        try
        {

            var products = await GetAll();

            var indexOfProduct = products.FindIndex(x => x.Id == product.Id);

            products[indexOfProduct] = product;

            await jsonFileStore.WriteAsync(productPath, products);

            return product;
        }
        
        finally
        {
            Lock.Release();
        }
    }

    public async Task<Product> Delete(Guid id)
    {
        await Lock.WaitAsync();

        try
        {
            var products = await GetAll();

            var deleteIndexOfProduct = products.FindIndex(x => x.Id == id);
            
            var product = products[deleteIndexOfProduct];

            products.Remove(product);

            await jsonFileStore.WriteAsync(productPath, products);

            return product;
        }
        
        finally
        {
            Lock.Release();
        }
    }
}