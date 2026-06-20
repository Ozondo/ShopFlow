using ShopFlow.ProductService.Endpoints;
using ShopFlow.ProductService.Infrastructure.Interfaces;
using ShopFlow.ProductService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddScoped<IProductRepository>(sp =>
            new ProductRepository(
                sp.GetRequiredService<IJsonFileStore>(),
                "Data/Products/products.json"));
builder.Services.AddSingleton<IJsonFileStore, JsonFileStore>();

var app = builder.Build();

app.MapGrpcService<ProductGrpcService>();
app.Run();