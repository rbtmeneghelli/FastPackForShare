namespace FastPackForShare.Models;

public sealed class CustomProduceResponseTypeModel<DTO> where DTO : class
{
    public int StatusCode { get; init; }
    public DTO Data { get; init; }
    public string Message { get; init; }
}
