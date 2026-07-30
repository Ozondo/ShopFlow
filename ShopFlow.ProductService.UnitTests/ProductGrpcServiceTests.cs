using FluentAssertions;
using MediatR;
using Moq;
using ShopFlow.Contracts.Product.V1;
using ShopFlow.ProductService.Endpoints;
using ShopFlow.ProductService.Usecase.CreateProduct;
using Xunit;
using Product = ShopFlow.ProductService.Domain.Products.Models.Product;

namespace ShopFlow.ProductService.UnitTests;

public class ProductGrpcServiceTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProductGrpcService _service;

    public ProductGrpcServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _service = new ProductGrpcService(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateProduct_Should_Return_Product()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Milk",
            "Food",
            100,
            10);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var request = new CreateProductRequest
        {
            Name = "Milk",
            Category = "Food",
            Price = "100",
            Stock = 10
        };

        // Act
        var result = await _service.CreateProduct(
            request,
            ServerCallContextMock.Create());

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id.ToString());
        result.Name.Should().Be("Milk");
        result.Category.Should().Be("Food");
        result.Price.Should().Be("100");
        result.Stock.Should().Be(10);

        _mediatorMock.Verify(x =>
            x.Send(
                It.Is<CreateProductCommand>(c =>
                    c.Name == "Milk" &&
                    c.Category == "Food" &&
                    c.Price == 100m &&
                    c.Stock == 10),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}