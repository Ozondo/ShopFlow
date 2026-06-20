using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopFlow.ProductService.Infrastructure.Interfaces;

public interface IJsonFileStore
{
    Task<List<T>> ReadAsync<T>(string path);

    Task WriteAsync<T>(string path, List<T> data);
}