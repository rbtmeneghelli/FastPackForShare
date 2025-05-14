using FastPackForShare.Enums;

namespace FastPackForShare.Interfaces;

public interface IRedisService : IDisposable
{
    Task AddDataString(string redisKey, string redisData, EnumRedisCacheTime enumRedisCacheTime = EnumRedisCacheTime.Medium);
    Task<string> GetDataString(string redisKey);
    Task AddDataObject<T>(string redisKey, T redisData, EnumRedisCacheTime enumRedisCacheTime = EnumRedisCacheTime.Medium) where T : class;
    Task<T> GetDataObject<T>(string redisKey) where T : class;
    Task RemoveData(string redisKey);
}
