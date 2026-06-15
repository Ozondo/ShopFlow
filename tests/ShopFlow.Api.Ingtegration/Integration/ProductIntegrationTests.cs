using ShopFlow.Api.Domain.Products.Models;
using ShopFlow.Api.Infrastructure;
using ShopFlow.Api.Infrastructure.Repositories;

namespace ShopFlow.Api.Tests.Integration;

public class ProductRepositoryTests : IDisposable
{
    private readonly string _path;
    private readonly JsonFileStore _store;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _path = Path.GetTempFileName();

        _store = new JsonFileStore();

        _repository = new ProductRepository(
            _store,
            _path);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllProducts()
    {
        var products = new List<Product>
        {
            CreateProduct(),
            CreateProduct()
        };

        await _store.WriteAsync(_path, products);

        var result = await _repository.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ShouldReturnProduct_WhenProductExists()
    {
        var product = CreateProduct();

        await _store.WriteAsync(
            _path,
            new List<Product> { product });

        var result = await _repository.GetById(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenProductDoesNotExist()
    {
        await _store.WriteAsync(
            _path,
            new List<Product>());

        var result = await _repository.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIds_ShouldReturnOnlyRequestedProducts()
    {
        var product1 = CreateProduct();
        var product2 = CreateProduct();
        var product3 = CreateProduct();

        await _store.WriteAsync(
            _path,
            new List<Product>
            {
                product1,
                product2,
                product3
            });

        var result = await _repository.GetByIds(
            [product1.Id, product3.Id]);

        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            x => x.Id == product1.Id);

        Assert.Contains(
            result,
            x => x.Id == product3.Id);
    }

    [Fact]
    public async Task Create_ShouldAddProduct()
    {
        var product = CreateProduct();

        await _store.WriteAsync(
            _path,
            new List<Product>());

        await _repository.Create(product);

        var result = await _repository.GetAll();

        Assert.Single(result);
        Assert.Equal(product.Id, result[0].Id);
    }

    [Fact]
    public async Task Update_ShouldUpdateProduct()
    {
        var product = CreateProduct();

        await _store.WriteAsync(
            _path,
            new List<Product> { product });

        var updatedProduct = product with
        {
            Name = "Updated Product",
            Category = "Updated Category",
            Price = 999,
            Stock = 50
        };

        await _repository.Update(updatedProduct);

        var result = await _repository.GetById(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Updated Product", result!.Name);
        Assert.Equal("Updated Category", result.Category);
        Assert.Equal(999, result.Price);
        Assert.Equal(50, result.Stock);
    }

    [Fact]
    public async Task Delete_ShouldRemoveProduct()
    {
        var product = CreateProduct();

        await _store.WriteAsync(
            _path,
            new List<Product> { product });

        var deletedProduct = await _repository.Delete(product.Id);

        var result = await _repository.GetAll();

        Assert.Empty(result);
        Assert.Equal(product.Id, deletedProduct.Id);
    }

    private static Product CreateProduct(
        string name = "Test Product",
        string category = "Test Category",
        decimal price = 100,
        int stock = 10)
    {
        return new Product(
            Guid.NewGuid(),
            name,
            category,
            price,
            stock);
    }

    public void Dispose()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }
}