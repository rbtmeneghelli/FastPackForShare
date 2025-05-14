using FastPackForShare.Enums;
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

    public async Task AddDataString(string redisKey, string redisData, EnumRedisCacheTime enumRedisCacheTime = EnumRedisCacheTime.Medium)
    {
        await _cache.SetStringAsync(redisKey, redisData, SetTimeToExpire(enumRedisCacheTime));
    }

    public async Task<string> GetDataString(string redisKey)
    {
        return await _cache.GetStringAsync(redisKey);
    }

    public async Task AddDataObject<T>(string redisKey, T redisData, EnumRedisCacheTime enumRedisCacheTime = EnumRedisCacheTime.Medium) where T : class
    {
        await _cache.SetStringAsync(redisKey, JsonSerializer.Serialize(redisData), SetTimeToExpire(enumRedisCacheTime));
    }

    public async Task<T> GetDataObject<T>(string redisKey) where T : class
    {
        var result = await _cache.GetStringAsync(redisKey);
        return result is not null ? JsonSerializer.Deserialize<T>(result) : default;
    }

    public async Task RemoveData(string redisKey)
    {
        await _cache.RemoveAsync(redisKey);
    }

    private DistributedCacheEntryOptions SetTimeToExpire(EnumRedisCacheTime enumRedisCacheTime)
    {
        DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
        cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes((int)enumRedisCacheTime));
        return cacheEntryOptions;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
