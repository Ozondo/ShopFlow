using Moq;

namespace ShopFlow.Api.Tests.Unit;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();

    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnProducts()
    {
        var products = new List<Product>
        {
            CreateProduct()
        };

        _productRepositoryMock
            .Setup(x => x.GetAll())
            .ReturnsAsync(products);
        
        var result = await _service.GetAll();
        
        Assert.True(result.Success);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetById_ShouldReturnProduct_WhenProductExists()
    {
        var product = CreateProduct();

        _productRepositoryMock
            .Setup(x => x.GetById(product.Id))
            .ReturnsAsync(product);
        
        var result = await _service.GetById(product.Id); 
        
        Assert.True(result.Success);
        Assert.Equal(product.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetById_ShouldFail_WhenProductNotFound()
    {
        var id = Guid.NewGuid();

        _productRepositoryMock
            .Setup(x => x.GetById(id))
            .ReturnsAsync((Product?)null);
        
        var result = await _service.GetById(id);
        
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Create_ShouldCreateProduct_WhenRequestIsValid()
    {
        var request = CreateProductRequest();

        _productRepositoryMock
            .Setup(x => x.Create(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);
        
        var result = await _service.Create(request);

        Assert.True(result.Success);

        _productRepositoryMock.Verify(
            x => x.Create(It.IsAny<Product>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenStockLessThanOne()
    {
        var request = CreateProductRequest(stock: 0);
        
        var result = await _service.Create(request);

        Assert.False(result.Success);

        _productRepositoryMock.Verify(
            x => x.Create(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenPriceLessOrEqualZero()
    {
        var request = CreateProductRequest(price: 0);
        
        var result = await _service.Create(request);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenNameIsEmpty()
    {
        var request = CreateProductRequest(name: "");
        
        var result = await _service.Create(request);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenCategoryIsEmpty()
    {
        var request = CreateProductRequest(category: "");
        
        var result = await _service.Create(request);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenProductNotFound()
    {
        var id = Guid.NewGuid();

        _productRepositoryMock
            .Setup(x => x.GetById(id))
            .ReturnsAsync((Product?)null);

        var request = CreateUpdateRequest();
        
        var result = await _service.Update(id, request);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenStockLessThanOne()
    {
        var product = CreateProduct();

        _productRepositoryMock
            .Setup(x => x.GetById(product.Id))
            .ReturnsAsync(product);

        var request = CreateUpdateRequest(stock: 0);
        
        var result = await _service.Update(product.Id, request);

        Assert.False(result.Success);

        _productRepositoryMock.Verify(
            x => x.Update(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenPriceLessOrEqualZero()
    {
        var product = CreateProduct();

        _productRepositoryMock
            .Setup(x => x.GetById(product.Id))
            .ReturnsAsync(product);

        var request = CreateUpdateRequest(price: 0);

        var result = await _service.Update(product.Id, request);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenNameIsEmpty()
    {
        var product = CreateProduct();

        _productRepositoryMock
            .Setup(x => x.GetById(product.Id))
            .ReturnsAsync(product);

        var request = CreateUpdateRequest(name: "");

        var result = await _service.Update(product.Id, request);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenCategoryIsEmpty()
    {
        var product = CreateProduct();

        _productRepositoryMock
            .Setup(x => x.GetById(product.Id))
            .ReturnsAsync(product);

        var request = CreateUpdateRequest(category: "");
        
        var result = await _service.Update(product.Id, request);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ShouldUpdateProduct_WhenRequestIsValid()
    {
        var product = CreateProduct();

        _productRepositoryMock
            .Setup(x => x.GetById(product.Id))
            .ReturnsAsync(product);

        _productRepositoryMock
            .Setup(x => x.Update(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        var request = CreateUpdateRequest();
        
        var result = await _service.Update(product.Id, request);
        
        Assert.True(result.Success);

        _productRepositoryMock.Verify(
            x => x.Update(It.IsAny<Product>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenProductNotFound()
    {
        var id = Guid.NewGuid();

        _productRepositoryMock
            .Setup(x => x.GetById(id))
            .ReturnsAsync((Product?)null);
        
        var result = await _service.Delete(id);
        
        Assert.False(result.Success);

        _productRepositoryMock.Verify(
            x => x.Delete(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_ShouldDeleteProduct_WhenProductExists()
    {
        var product = CreateProduct();

        _productRepositoryMock
            .Setup(x => x.GetById(product.Id))
            .ReturnsAsync(product);

        _productRepositoryMock
            .Setup(x => x.Delete(product.Id))
            .ReturnsAsync(product);
        
        var result = await _service.Delete(product.Id);
        
        Assert.True(result.Success);

        _productRepositoryMock.Verify(
            x => x.Delete(product.Id),
            Times.Once);
    }

    private static Product CreateProduct(
        int stock = 10,
        decimal price = 100)
    {
        return new Product(
            Guid.NewGuid(),
            "Test Product",
            "Test Category",
            price,
            stock);
    }

    private static CreateProductRequest CreateProductRequest(
        string name = "Test Product",
        string category = "Test Category",
        decimal price = 100,
        int stock = 10)
    {
        return new CreateProductRequest(
            name,
            category,
            price,
            stock);
    }

    private static UpdateProductRequest CreateUpdateRequest(
        string name = "Updated Product",
        string category = "Updated Category",
        decimal price = 200,
        int stock = 20)
    {
        return new UpdateProductRequest(
            name,
            category,
            price,
            stock);
    }
}