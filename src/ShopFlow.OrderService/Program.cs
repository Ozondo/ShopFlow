using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ShopFlow.Common;
using ShopFlow.Contracts.Product.V1;
using ShopFlow.OrderService.Endpoints;
using ShopFlow.OrderService.Infrastructure.Interfaces;
using ShopFlow.OrderService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddScoped<IOrdersRepository, OrderRepository>();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider
        .GetRequiredService<IOptions<MongoDbSettings>>()
        .Value;

    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddGrpcClient<Product.ProductClient>(options =>
{
    options.Address = new Uri("http://productservice:8080");
});

var app = builder.Build();

app.MapGrpcService<OrderGrpcService>();
app.Run();