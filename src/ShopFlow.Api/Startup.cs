using ShopFlow.Contracts.Product.V1;
using ShopFlow.Contracts.Order.V1;

namespace ShopFlow.Api;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen();
        
        services.AddGrpcClient<Order.OrderClient>(options =>
        {
            options.Address = new Uri(Configuration["OrderService:Url"]!);
        });
        
        services.AddGrpcClient<Product.ProductClient>(options =>
        {
            options.Address = new Uri(Configuration["ProductService:Url"]!);
        });
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
