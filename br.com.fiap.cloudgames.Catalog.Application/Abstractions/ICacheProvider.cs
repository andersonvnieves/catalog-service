using System.Runtime.CompilerServices;

namespace br.com.fiap.cloudgames.Catalog.Application.Abstractions
{
    public interface ICacheProvider
    {
        const string UPDATING_LOCK = "UPDATING_LOCK";

        Task<bool> ContainsKeyAsync(string key);
        Task<T?> GetDataAsync<T>(string key);
        Task<Dictionary<string, T?>> GetBatchDataAsync<T>(IEnumerable<string> keys);
        Task SetDataAsync<T>(string key, T value);
        Task SetDataAsync<T>(string key, T value, TimeSpan ttl);
        Task SetBatchDataAsync<T>(Dictionary<string, T> items);
        Task SetBatchDataAsync<T>(Dictionary<string, T> items, TimeSpan ttl);
        Task<bool> DeleteDataAsync(string key);
    }
}
