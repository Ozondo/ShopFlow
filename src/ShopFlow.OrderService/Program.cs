using ShopFlow.Api.Infrastructure.Interfaces;
using ShopFlow.Contracts.Product.V1;
using ShopFlow.OrderService.Endpoints;
using ShopFlow.OrderService.Infrastructure.Interfaces;
using ShopFlow.OrderService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddScoped<IOrdersRepository>(sp =>
    new OrderRepository(
        sp.GetRequiredService<IJsonFileStore>(),
        "Data/Orders/orders.json"));
builder.Services.AddSingleton<IJsonFileStore, JsonFileStore>();

builder.Services.AddGrpcClient<Product.ProductClient>(options =>
{
    options.Address = new Uri("http://localhost:5249");
});

var app = builder.Build();

app.MapGrpcService<OrderGrpcService>();
app.Run();