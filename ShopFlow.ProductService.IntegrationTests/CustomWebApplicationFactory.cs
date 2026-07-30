using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ShopFlow.ProductService.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public string DatabaseName { get; } = $"ShopFlowTests_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MongoDb:ConnectionString"] = "mongodb://admin:admin123@localhost:27017",
                ["MongoDb:DatabaseName"] = DatabaseName
            });
        });
    }

    public IMongoDatabase GetDatabase()
    {
        var client = new MongoClient("mongodb://admin:admin123@localhost:27017");
        return client.GetDatabase(DatabaseName);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                var client = new MongoClient("mongodb://admin:admin123@localhost:27017");
                client.DropDatabase(DatabaseName);
            }
            catch
            {
                // ignore
            }
        }

        base.Dispose(disposing);
    }
}