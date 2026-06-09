using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Infrastructure;
using ShopFlow.Api.Infrastructure.Repositories;

namespace ShopFlow.Api;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        

        // TODO: подключи сваггер
        services.AddSwaggerGen();

        // TODO: зарегистрируй Infrastructure-сервисы (JsonFileStore, репозитории)
        services.AddSingleton<JsonFileStore>();
        
        services.AddScoped<IProductRepository>(sp => 
            new ProductRepository(
                sp.GetRequiredService<JsonFileStore>(),
                "Data/Products/products.json"));

        services.AddScoped<IOrdersRepository>(sp =>
            new OrderRepository(
                sp.GetRequiredService<JsonFileStore>(),
                "Data/Orders/orders.json"));
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
