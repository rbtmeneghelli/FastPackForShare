using FastPackForShare.Extensions;
using FastPackForShare.Interfaces;
using FastPackForShare.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FastPackForShare.Services;

public class MemoryCacheService<TData> : IMemoryCacheService<TData> where TData : class
{
    private readonly IMemoryCache _iMemoryCache;
    private MemoryCacheEntryOptions _cacheOptions { get; set; }

    public MemoryCacheService(IMemoryCache iMemoryCache)
    {
        _iMemoryCache = iMemoryCache;
    }

    public void SetMemoryCacheEntryOptions(MemoryCacheModel memoryCacheModel)
    {
        _cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddHours(memoryCacheModel.AbsoluteExpirationInHours),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromMinutes(memoryCacheModel.SlidingExpirationInMinutes),
            Size = 1024
        };
    }

    public bool TryGet(string cacheKey, out TData data)
    {
        _iMemoryCache.TryGetValue(cacheKey, out data);
        return GuardClauseExtension.IsNull(data);
    }

    public TData Set(string cacheKey, TData value)
    {
        return _iMemoryCache.Set(cacheKey, value, _cacheOptions);
    }

    public TData Get(string cacheKey)
    {
        return _iMemoryCache.Get<TData>(cacheKey);
    }

    public void Remove(string cacheKey)
    {
        _iMemoryCache.Remove(cacheKey);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
