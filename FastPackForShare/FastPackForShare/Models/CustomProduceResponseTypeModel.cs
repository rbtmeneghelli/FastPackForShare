namespace FastPackForShare.Models;

public sealed class CustomValidResponseTypeModel<DTO> where DTO : class
{
    public required int StatusCode { get; init; }
    public required DTO Data { get; init; }
    public required string Message { get; init; }
}

public sealed class CustomInValidResponseTypeModel
{
    public required int StatusCode { get; init; }
    public required string Message { get; init; }
}