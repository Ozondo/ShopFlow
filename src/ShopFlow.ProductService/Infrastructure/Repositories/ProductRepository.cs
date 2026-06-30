using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ShopFlow.Common;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;

namespace ShopFlow.ProductService.Infrastructure.Repositories;

public class ProductRepository: IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _products = database.GetCollection<Product>("Products");
    }
    
    public async Task<List<Product>?> GetAll()
    {
        return await _products.Find(x => true).ToListAsync();
    }

    public async Task<Product?> GetById(Guid id)
    {
        return await _products.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
    
    public async Task<List<Product>> GetByIds(IEnumerable<Guid> ids)
    {
        var idsList = ids.ToHashSet();
        return await _products.Find(x => idsList.Contains(x.Id)).ToListAsync();
    }
    
    public async Task<Product> Create(Product product)
    {
        await _products.InsertOneAsync(product);
        return product;
    }

    public async Task<Product> Update(Product product)
    {
        await _products.ReplaceOneAsync(x => x.Id == product.Id, product);
        return product;
    }

    public async Task<Product> Delete(Guid id)
    {
        var result = await _products.Find(x => x.Id == id).FirstOrDefaultAsync();
        await _products.DeleteOneAsync(x => x.Id == id);
        
        return result;
    }
}