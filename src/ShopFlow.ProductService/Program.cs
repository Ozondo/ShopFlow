using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ShopFlow.Common;
using ShopFlow.ProductService.Endpoints;
using ShopFlow.ProductService.Infrastructure.Interfaces;
using ShopFlow.ProductService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider
        .GetRequiredService<IOptions<MongoDbSettings>>()
        .Value;

    return new MongoClient(settings.ConnectionString);
});

var app = builder.Build();

app.MapGrpcService<ProductGrpcService>();
app.Run();