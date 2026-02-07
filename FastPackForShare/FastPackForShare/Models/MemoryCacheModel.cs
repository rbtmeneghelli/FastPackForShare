namespace FastPackForShare.Models;

public record MemoryCacheModel
{
    public required int AbsoluteExpirationInHours { get; init; }
    public required int SlidingExpirationInMinutes { get; init; }
}
