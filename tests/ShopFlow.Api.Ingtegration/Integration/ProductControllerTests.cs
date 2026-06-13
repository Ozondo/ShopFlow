using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ShopFlow.Api.Tests.Integration;

public class ProductsControllerTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/products");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            $"/api/products/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenRequestIsValid_ReturnsCreated()
    {
        var request = new
        {
            Name = "iPhone",
            Category = "Phone",
            Price = 100,
            Stock = 10
        };

        var response = await _client.PostAsJsonAsync(
            "/api/products/create",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenPriceIsNegative_ReturnsBadRequest()
    {
        var request = new
        {
            Name = "iPhone",
            Category = "Phone",
            Price = -100,
            Stock = 10
        };

        var response = await _client.PostAsJsonAsync(
            "/api/products/create",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenStockIsZero_ReturnsBadRequest()
    {
        var request = new
        {
            Name = "iPhone",
            Category = "Phone",
            Price = 100,
            Stock = 0
        };

        var response = await _client.PostAsJsonAsync(
            "/api/products/create",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync(
            $"/api/products/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var request = new
        {
            Name = "Samsung",
            Category = "Phone",
            Price = 200,
            Stock = 10
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/products/{Guid.NewGuid()}",
            request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}