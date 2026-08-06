using System.Text.Json;
using StackExchange.Redis;

namespace ShopFlow.Common.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json);
        
        if (expiration.HasValue)
        {
            await _database.KeyExpireAsync(key, expiration.Value);
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var result = await _database.StringGetAsync(key);

        if (result.IsNullOrEmpty)
        {
            return default;
        }
        
        return JsonSerializer.Deserialize<T>(result!);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}