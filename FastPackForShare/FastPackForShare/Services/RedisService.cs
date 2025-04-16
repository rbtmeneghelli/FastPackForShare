
using FastPackForShare.Interfaces;
using FastPackForShare.Services.Bases;
using Microsoft.Extensions.Caching.Distributed;

namespace FastPackForShare.Services;

public sealed class RedisService : BaseHandlerService, IRedisService
{
    private readonly IDistributedCache _cache;

    public RedisService(IDistributedCache cache, INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
        _cache = cache;
    }

    public async Task AddDataString(string redisKey, string redisData)
    {
        await _cache.SetStringAsync(redisKey, redisData, SetTimeToExpire());
    }

    public async Task<string> GetDataString(string redisKey)
    {
        var result = await _cache.GetStringAsync(redisKey);
        return result;
    }

    public async Task AddDataObject<T>(string redisKey, T redisData) where T : class
    {
        await _cache.SetStringAsync(redisKey, JsonSerializer.Serialize(redisData), SetTimeToExpire());
    }

    public async Task<T> GetDataObject<T>(string redisKey) where T : class
    {
        var result = await _cache.GetStringAsync(redisKey);
        return JsonSerializer.Deserialize<T>(result);
    }

    private DistributedCacheEntryOptions SetTimeToExpire()
    {
        DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
        cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
        return cacheEntryOptions;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
