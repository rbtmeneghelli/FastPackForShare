namespace FastPackForShare.Models;

public sealed class CustomValidResponseTypeModel<DTO> where DTO : class
{
    public int StatusCode { get; init; }
    public DTO Data { get; init; }
    public string Message { get; init; }
}

public sealed class CustomInValidResponseTypeModel
{
    public int StatusCode { get; init; }
    public string Message { get; init; }
}