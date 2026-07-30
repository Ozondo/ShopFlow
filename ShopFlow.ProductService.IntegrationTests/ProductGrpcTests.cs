using FluentAssertions;
using Grpc.Net.Client;
using ShopFlow.Contracts.Product.V1;
using Xunit;

namespace ShopFlow.ProductService.IntegrationTests;

public class ProductGrpcTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly Product.ProductClient _client;

    public ProductGrpcTests(CustomWebApplicationFactory factory)
    {
        var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, new GrpcChannelOptions
        {
            HttpHandler = factory.Server.CreateHandler()
        });

        _client = new Product.ProductClient(channel);
    }

    [Fact]
    public async Task CreateProduct_Should_Create_Product()
    {
        var response = await _client.CreateProductAsync(new CreateProductRequest
        {
            Name = "Milk",
            Category = "Food",
            Price = "100",
            Stock = 10
        });

        response.Should().NotBeNull();
        response.Name.Should().Be("Milk");
    }
}