using ShopFlow.Api.Domain.Products.Models;
using ShopFlow.Api.Infrastructure;
using ShopFlow.Api.Infrastructure.Repositories;

namespace ShopFlow.Api.Tests.Unit;

public class ProductRepositoryTests
{
    private Product CreateProduct()
    {
        return new Product(
            Guid.NewGuid(),
            "iPhone",
            "Smartphones",
            100,
            10);
    }
    
    [Fact]
    public async Task GetAll_WhenDataIsEmpty_ReturnsEmptyList()
    {
        var path = Path.GetTempFileName();

        var repository = new ProductRepository(
            new JsonFileStore(),
            path);
        
        var result = await repository.GetAll();
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetAll_WhenDataIsNotEmpty_ReturnsList()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        await jsonFileStore.WriteAsync(
            path,
            new List<Product>
            {
                CreateProduct(),
                CreateProduct(),
            });
        
        
        var repository = new ProductRepository(
            jsonFileStore,
            path);
        
        var result = await repository.GetAll();
        
        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public async Task GetById_WhenDataIsEmpty_ReturnsNull()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        var repository = new ProductRepository(
            jsonFileStore,
            path);
        
        var result = await repository.GetById(Guid.Empty);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetById_WhenDataIsNotEmpty_ReturnsProduct()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        var repository = new ProductRepository(
            jsonFileStore,
            path);
        
        var product = CreateProduct();
        
        await jsonFileStore.WriteAsync(
            path,
            new List<Product>
            {
                product
            });
        
        var result = await repository.GetById(product.Id);
        
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }
    
    [Fact]
    public async Task Create_WhenProductIsValid_ReturnsProduct()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        var repository = new ProductRepository(
            jsonFileStore,
            path);
        
        var product = CreateProduct();
        
        await repository.Create(product);
        
        var products = await repository.GetAll();
        
        Assert.NotNull(products);
        Assert.Equal(product.Id, products[0].Id);
    }
    
    [Fact]
    public async Task Update_WhenProductExists_ReturnsProduct()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        var repository = new ProductRepository(
            jsonFileStore,
            path);
        
        var product = CreateProduct();
        
        await repository.Create(product);
        
        var updatedProduct = new Product(
            product.Id,
            "Samsung",
            product.Category,
            200,
            100
            );
        
        await repository.Update(product.Id,updatedProduct);
        
        var result = await repository.GetById(product.Id);
        
        Assert.NotNull(result);
        Assert.Equal(updatedProduct.Id, result?.Id);
        Assert.Equal(updatedProduct.Category, result?.Category);
        Assert.Equal(updatedProduct.Name, result?.Name);
        Assert.Equal(updatedProduct.Price, result?.Price);
        Assert.Equal(updatedProduct.Stock, result?.Stock);
    }
    
    [Fact]
    public async Task Update_WhenProductDoesNotExists_ReturnsNull()
    {
        var path = Path.GetTempFileName();
        var jsonFileStore = new JsonFileStore();
        
        var repository = new ProductRepository(
            jsonFileStore,
            path);
        
        var product = CreateProduct();
        
        await repository.Create(product);
        
        var updatedProduct = new Product(
            Guid.NewGuid(),
            "Samsung",
            product.Category,
            200,
            100
        );
        
        var result = await repository.Update(updatedProduct.Id, updatedProduct);
        
        Assert.Null(result);
    }
}