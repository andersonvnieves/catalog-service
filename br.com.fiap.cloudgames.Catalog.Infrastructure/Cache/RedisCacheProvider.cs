using br.com.fiap.cloudgames.Catalog.Application.Abstractions;
using StackExchange.Redis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Cache
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly IDatabase _database;
        private readonly JsonSerializerOptions _serializerOptions;

        public RedisCacheProvider(IDatabase database) { 
            _database = database;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = { EnablePrivateSettersAndConstructors }
                }
            };
        }

        private static void EnablePrivateSettersAndConstructors(JsonTypeInfo typeInfo)
        {
            if (typeInfo.Kind != JsonTypeInfoKind.Object)
                return;

            foreach (var property in typeInfo.Properties)
            {
                if (property.Set == null)
                {
                    var propInfo = typeInfo.Type.GetProperty(property.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (propInfo?.SetMethod != null)
                    {
                        property.Set = (obj, value) => propInfo.SetValue(obj, value);
                    }
                }
            }
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            string stringValue = value.ToString();
            if (stringValue == ICacheProvider.UPDATING_LOCK)
                return default;
            
            return JsonSerializer.Deserialize<T>(value.ToString(), _serializerOptions);            
        }

        public async Task<Dictionary<string, T?>> GetBatchDataAsync<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            if (redisKeys.Length == 0)
                return new Dictionary<string, T?>();

            var values = await _database.StringGetAsync(redisKeys);
            var result = new Dictionary<string, T?>();

            for (int i = 0; i < redisKeys.Length; i++)
            {
                var val = values[i];
                string keyStr = keys.ElementAt(i);

                if (val.IsNullOrEmpty)
                {
                    result[keyStr] = default;
                    continue;
                }

                string stringValue = val.ToString();

                if (stringValue == ICacheProvider.UPDATING_LOCK)
                {
                    result[keyStr] = default;
                    continue;
                }

                result[keyStr] = JsonSerializer.Deserialize<T>(stringValue, _serializerOptions);
            }

            return result;
        }

        public async Task SetDataAsync<T>(string key, T value)
        {
            var stringValue = JsonSerializer.Serialize<T>(value);
            await _database.StringSetAsync(key, stringValue);
        }

        public async Task SetDataAsync<T>(string key, T value, TimeSpan ttl)
        {
            var stringValue = JsonSerializer.Serialize<T>(value);
            await _database.StringSetAsync(key, stringValue, ttl);
        }

        public async Task SetBatchDataAsync<T>(Dictionary<string, T> items)
        {
            var batch = _database.CreateBatch();
            var tasks = new List<Task>();

            foreach (var kvp in items)
            {
                var stringValue = JsonSerializer.Serialize(kvp.Value);
                tasks.Add(batch.StringSetAsync(kvp.Key, stringValue));
            }

            batch.Execute();
            await Task.WhenAll(tasks);
        }

        public async Task SetBatchDataAsync<T>(Dictionary<string, T> items, TimeSpan ttl)
        {
            var batch = _database.CreateBatch();
            var tasks = new List<Task>();

            foreach (var kvp in items)
            {
                var stringValue = JsonSerializer.Serialize(kvp.Value);
                tasks.Add(batch.StringSetAsync(kvp.Key, stringValue, ttl));
            }

            batch.Execute();
            await Task.WhenAll(tasks);
        }

        public async Task<bool> DeleteDataAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }        
    }
}
