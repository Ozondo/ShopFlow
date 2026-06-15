using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Application.Services;
using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Api.Infrastructure;
using ShopFlow.Api.Infrastructure.Interfaces;
using ShopFlow.Api.Infrastructure.Repositories;

namespace ShopFlow.Api;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen();
        
        services.AddSingleton<IJsonFileStore, JsonFileStore>();
        
        services.AddScoped<IProductRepository>(sp => 
            new ProductRepository(
                sp.GetRequiredService<IJsonFileStore>(),
                "Data/Products/products.json"));

        services.AddScoped<IOrdersRepository>(sp =>
            new OrderRepository(
                sp.GetRequiredService<IJsonFileStore>(),
                "Data/Orders/orders.json"));

        services.AddScoped<IProductSevice, ProductService>();
        
        services.AddScoped<IOrderService, OrderService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
