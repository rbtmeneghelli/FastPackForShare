using System.Threading.RateLimiting;

namespace FastPackForShare.Helpers;

public static class HelperRateLimiter
{
    /// <summary>
    /// Metódo para criar a chave que será usada a partir do metódo ValidateRateLimitRequest
    /// </summary>
    /// <param name="key">default</param>
    /// <param name="intervalInSeconds">120</param>
    /// <param name="validTokens">1</param>
    /// <returns></returns>
    public static PartitionedRateLimiter<string> GetPartitionedRateLimiter(string key = "default", int intervalInSeconds = 120, int validTokens = 1)
    {
        return PartitionedRateLimiter.Create<string, string>(_ =>
        RateLimitPartition.GetTokenBucketLimiter(
        partitionKey: key,
        factory: _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = validTokens,
            TokensPerPeriod = validTokens,
            ReplenishmentPeriod = TimeSpan.FromSeconds(intervalInSeconds),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true
        }));
    }

    /// <summary>
    /// Metódo para validar o uso do endpoint requisitado, após primeira solicitação
    /// </summary>
    /// <param name="rateLimiter"></param>
    /// <returns></returns>
    public static async Task<bool> ValidateRateLimitRequest(PartitionedRateLimiter<string> rateLimiter)
    {
        using var lease = await rateLimiter.AcquireAsync("default");
        return !lease.IsAcquired;
    }
}
