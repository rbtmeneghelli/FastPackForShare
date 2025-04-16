namespace FastPackForShare.Interfaces;

public interface IRedisService : IDisposable
{
    Task AddDataString(string redisKey, string redisData);
    Task<string> GetDataString(string redisKey);
    Task AddDataObject<T>(string redisKey, T redisData) where T : class;
    Task<T> GetDataObject<T>(string redisKey) where T : class;
}
