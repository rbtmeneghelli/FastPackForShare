using FastPackForShare.Models;

namespace FastPackForShare.Services;

public interface IMemoryCacheService<TData> : IDisposable where TData : class
{
    void SetMemoryCacheEntryOptions(MemoryCacheModel memoryCacheModel);
    bool TryGet(string cacheKey, out TData data);
    TData Set(string cacheKey, TData data);
    TData Get(string cacheKey);
    void Remove(string cacheKey);
}
