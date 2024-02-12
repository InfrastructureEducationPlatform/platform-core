using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BlockInfrastructure.Common.Services;

public interface ICacheService
{
    public Task<T?> GetAsync<T>(string key);
    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    public Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);
    public Task DeleteAsync(string key);
}

public class CacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<CacheService> logger) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var database = connectionMultiplexer.GetDatabase();
        var value = await database.StringGetAsync(key);
        return value.HasValue ? JsonConvert.DeserializeObject<T>(value.ToString()) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var database = connectionMultiplexer.GetDatabase();
        await database.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry ?? TimeSpan.FromMinutes(10));
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var database = connectionMultiplexer.GetDatabase();
        var value = await database.StringGetAsync(key);
        if (value.HasValue)
        {
            logger.LogInformation("Cache Hit for {Key}", key);
            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        // Cache Missed
        logger.LogInformation("Cache Missed for {Key}", key);
        var newValue = await factory();
        await database.StringSetAsync(key, JsonConvert.SerializeObject(newValue), expiry ?? TimeSpan.FromMinutes(10));
        return newValue;
    }

    public async Task DeleteAsync(string key)
    {
        var database = connectionMultiplexer.GetDatabase();
        await database.KeyDeleteAsync(key);
    }
}