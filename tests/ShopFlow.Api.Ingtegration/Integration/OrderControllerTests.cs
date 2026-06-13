using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ShopFlow.Api.Tests.Integration;

public class OrdersControllerTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrdersControllerTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/orders");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenOrderDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            $"/api/orders/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var request = new
        {
            CustomerName = "Egor",
            Items = new[]
            {
                new
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 1
                }
            }
        };

        var response = await _client.PostAsJsonAsync(
            "/api/orders",
            request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenOrderDoesNotExist_ReturnsNotFound()
    {
        var request = new
        {
            OrderStatus = 1
        };

        var response = await _client.PatchAsJsonAsync(
            $"/api/orders/{Guid.NewGuid()}",
            request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenStatusIsInvalid_ReturnsBadRequest()
    {
        var request = new
        {
            OrderStatus = 999
        };

        var response = await _client.PatchAsJsonAsync(
            $"/api/orders/{Guid.NewGuid()}",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}