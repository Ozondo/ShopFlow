namespace ShopFlow.Api.Infrastructure;

public interface IJsonFileStore
{
    Task<List<T>> ReadAsync<T>(string path);

    Task WriteAsync<T>(string path, List<T> data);
}