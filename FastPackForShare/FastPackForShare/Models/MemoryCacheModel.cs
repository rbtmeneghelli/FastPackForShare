namespace FastPackForShare.Models;

public record MemoryCacheModel
{
    public int AbsoluteExpirationInHours { get; init; }
    public int SlidingExpirationInMinutes { get; init; }
}
