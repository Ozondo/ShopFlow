using System.Text.Json;

namespace ShopFlow.Api.Infrastructure;

public class JsonFileStore: IJsonFileStore
{
    public async Task<List<T>> ReadAsync<T>(string path)
    {
        if (!File.Exists(path))
            return [];

        var json = await File.ReadAllTextAsync(path);
        
        if (string.IsNullOrWhiteSpace(json))
            return [];

        return JsonSerializer.Deserialize<List<T>>(
                   json,
                   new JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   })
               ?? [];
    }

    public async Task WriteAsync<T>(string path, List<T> data)
    {
        var json = JsonSerializer.Serialize(
            data,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        await File.WriteAllTextAsync(path, json);
    }
}