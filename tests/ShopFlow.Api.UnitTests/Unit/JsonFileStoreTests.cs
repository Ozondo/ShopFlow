using ShopFlow.Api.Infrastructure;

namespace ShopFlow.Api.Tests.Unit;

public class JsonFileStoreTests
{
    private sealed record TestEntity(Guid Id);

    
    [Fact]
    public async Task ReadAsync_WhenFileDoesNotExist_ReturnsEmptyList()
    {
        var store = new JsonFileStore();

        var result = await store.ReadAsync<string>("file.json");
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task ReadAsync_WhenFileExists_ReturnsData()
    {
        var store = new JsonFileStore();
        
        var path = Path.GetTempFileName();
        
        await File.WriteAllTextAsync(
            path,
            """
            [
                {
                    "id":"11111111-1111-1111-1111-111111111111"
                }
            ]
            """);
        
        var result = await store.ReadAsync<TestEntity>(path);
        
        Assert.Single(result);
        Assert.Equal(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            result[0].Id);
    }
    
    [Fact]
    public async Task WriteAsync_WhenFileExists_ReturnsData()
    {
        var store = new JsonFileStore();
        
        var path = Path.GetTempFileName();

        var data = new List<TestEntity>()
        {
            new TestEntity(Guid.Parse("11111111-1111-1111-1111-111111111111")),
        };
        
        await store.WriteAsync(path, data);
        
        var result = await store.ReadAsync<TestEntity>(path);
        
        Assert.Single(result);
        Assert.Equal(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            result[0].Id);
    }
    
}